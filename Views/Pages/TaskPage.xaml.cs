using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;
using TaskForge.ViewModels;
using TaskForge.Views.Dialogs;
using TaskForge.Helpers;


namespace TaskForge.Views.Pages
{
    public partial class TaskPage : Page, INotifyPropertyChanged
    {
        private readonly IUserSession _userSession;
        private readonly ApplicationDBContext _dbContext;
        private ObservableCollection<TaskItem> _tasks;
        private DispatcherTimer _timer;
        private int? _activeTaskId;
        private DateTime _timerStartTime;
        private int _accumulatedSeconds;
        public ObservableCollection<TaskItem> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(nameof(Tasks)); }
        }

        public ICommand AddTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand CompleteTaskCommand { get; }
        public ICommand ToggleTimerCommand { get; }

        public TaskPage(ApplicationDBContext dbContext, IUserSession userSession)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _userSession = userSession;
            Tasks = new ObservableCollection<TaskItem>();
            DataContext = this;

            AddTaskCommand = new RelayCommand(OnAddTask);
            EditTaskCommand = new RelayCommand<TaskItem>(OnEditTask);
            CompleteTaskCommand = new RelayCommand<TaskItem>(OnCompleteTask);
            ToggleTimerCommand = new RelayCommand<TaskItem>(OnToggleTimer);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += TimerTick;

            Loaded += async (s, e) => await LoadTasksAsync();
        }

        private async Task LoadTasksAsync()
        {
            var tasksFromDb = await _dbContext.Tasks
                .Where(t => t.User_id == _userSession.CurrentUserId && t.Status != "Completed")
                .OrderByDescending(t => t.Created_at)
                .ToListAsync();

            Tasks.Clear();
            foreach (var task in tasksFromDb)
                Tasks.Add(task);
        }
        private void OnAddTask()
        {
            var viewModel = new TaskEditViewModel(_dbContext, _userSession.CurrentUserId);
            var window = new TaskEditWindow(viewModel);
            window.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            var result = window.ShowDialog();
            if (result == true)
                _ = LoadTasksAsync();
        }

        private void OnEditTask(TaskItem task)
        {
            if (task == null) return;
            var viewModel = new TaskEditViewModel(_dbContext, _userSession.CurrentUserId, task);
            var window = new TaskEditWindow(viewModel);
            window.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            var result = window.ShowDialog();
            if (result == true)
                _ = LoadTasksAsync();
        }

        private async void OnCompleteTask(TaskItem task)
        {
            if (task == null || task.Status == "Completed") return;

            if (_activeTaskId == task.Id)
            {
                StopTimerAndSave();
            }

            // Обновляем статус задачи
            task.Status = "Completed";
            task.Update_at = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            // Начисляем опыт пользователю
            var user = await _dbContext.Users.FindAsync(task.User_id);
            if (user != null)
            {
                user.Total_exp += task.Reward_exp;
                user.Level = user.Total_exp / 100; // 100 опыта = 1 уровень
                user.Updated_at = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }

            // Проверка достижений
            await CheckAndUnlockAchievements(task.User_id);
            // Обновляем список задач
            await LoadTasksAsync();

            RefreshCompletedTasksInUserPage();
            await LoadTasksAsync();
        }

        private void RefreshCompletedTasksInUserPage()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow?.MainFrame?.Content is UserPage userPage)
            {
                userPage.RefreshCompletedTask();
            }
        }
        private void OnToggleTimer(TaskItem task)
        {
            if (task == null) return;
            if (_activeTaskId == task.Id)
            {
                StopTimerAndSave();
            }
            else
            {
                if (_activeTaskId.HasValue)
                {
                    StopTimerAndSave();
                }
                StartTimer(task);
            }
        }

        private void StartTimer(TaskItem task)
        {
            _activeTaskId = task.Id;
            _accumulatedSeconds = task.Actual_time;
            _timerStartTime = DateTime.Now;
            _timer.Start();

        }

        private async Task StopTimerAndSave()
        {
            if (!_activeTaskId.HasValue) return;
            _timer?.Stop();

            var elapsed = (int)(DateTime.Now - _timerStartTime).TotalSeconds;
            _accumulatedSeconds += elapsed;

            var task = Tasks.FirstOrDefault(t => t.Id == _activeTaskId.Value);
            if (task != null)
            {
                task.Actual_time = _accumulatedSeconds;
                OnPropertyChanged(nameof(Task));

                var dbTask = await _dbContext.Tasks.FindAsync(_activeTaskId.Value);
                if (dbTask != null)
                {
                    dbTask.Actual_time = _accumulatedSeconds;
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (!_activeTaskId.HasValue) return;
            var elapsed = (int)(DateTime.Now - _timerStartTime).TotalSeconds;
            var total = _accumulatedSeconds + elapsed;

            var task = Tasks.FirstOrDefault(t => t.Id == _activeTaskId.Value);
            if (task != null)
            {
                task.Actual_time = total;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        private async Task CheckAndUnlockAchievements(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            var completedTasksCount = await _dbContext.Tasks.CountAsync(t => t.User_id == userId && t.Status == "Completed");
            var unlockedAchievements = await _dbContext.Users_Achievements
                .Where(ua => ua.User_id == userId)
                .Select(ua => ua.Achievement_id)
                .ToListAsync();
            var allAchievements = await _dbContext.Achievements.ToListAsync();

            bool changed = false;
            foreach (var ach in allAchievements)
            {
                if (unlockedAchievements.Contains(ach.Id)) continue;
                bool conditionMet = false;

                // Простой парсинг критерия (формат "TasksCompleted >= 10" или "Exp >= 500")
                if (ach.Criteria.StartsWith("TasksCompleted"))
                {
                    var value = int.Parse(ach.Criteria.Split(">=")[1].Trim());
                    conditionMet = completedTasksCount >= value;
                }
                else if (ach.Criteria.StartsWith("Exp"))
                {
                    var value = int.Parse(ach.Criteria.Split(">=")[1].Trim());
                    conditionMet = user.Total_exp >= value;
                }

                if (conditionMet)
                {
                    _dbContext.Users_Achievements.Add(new User_Achievement
                    {
                        User_id = userId,
                        Achievement_id = ach.Id,
                        Earned_at = DateTime.Now
                    });
                    changed = true;
                }
            }

            if (changed)
                await _dbContext.SaveChangesAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnAddTask();
        }
    }
}
