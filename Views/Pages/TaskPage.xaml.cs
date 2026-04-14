using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;
using TaskForge.ViewModels;
using TaskForge.Views.Dialogs;


namespace TaskForge.Views.Pages
{
    public partial class TaskPage : Page, INotifyPropertyChanged
    {
        private readonly IUserSession _userSession;
        private readonly ApplicationDBContext _dbContext;
        private ObservableCollection<TaskItem> _tasks;
        public ObservableCollection<TaskItem> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(nameof(Tasks)); }
        }

        public ICommand AddTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand CompleteTaskCommand { get; }

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

            Loaded += async (s, e) => await LoadTasksAsync();
        }

        private async Task LoadTasksAsync()
        {
            var tasksFromDb = await _dbContext.Tasks
                .Where(t => t.User_id == _userSession.CurrentUserId)
                .OrderByDescending(t=>t.Created_at)
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
    public class RelayCommand : ICommand
    {
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public void Execute(object parameter) => _execute();
        public event EventHandler CanExecuteChanged;
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);
        public void Execute(object parameter) => _execute((T)parameter);
        public event EventHandler CanExecuteChanged;
    }

}
