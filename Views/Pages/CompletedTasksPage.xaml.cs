using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskForge.Helpers;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;

namespace TaskForge.Views.Pages
{
    public partial class CompletedTasksPage : Page, INotifyPropertyChanged
    {
        private readonly ApplicationDBContext _context;
        private readonly IUserSession _userSession;
        private ObservableCollection<TaskItem> _completedTasks;
        public ICommand RestoreTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ObservableCollection<TaskItem> CompletedTasks
        {
            get => _completedTasks;
            set { _completedTasks = value; OnPropertyChanged(nameof(CompletedTasks)); }
        }

        public CompletedTasksPage(ApplicationDBContext context, IUserSession userSession)
        {
            InitializeComponent();
            _context = context;
            _userSession = userSession;
            CompletedTasks = new ObservableCollection<TaskItem>();
            DataContext = this;
            Loaded += async (s, e) => await LoadCompletedTasksAsync();

            RestoreTaskCommand = new RelayCommand<TaskItem>(OnRestoreTask);
            DeleteTaskCommand = new RelayCommand<TaskItem>(OnDeleteCompletedTask);
        }

        public async Task LoadCompletedTasksAsync()
        {
            var tasks = await _context.Tasks
                .Where(t => t.User_id == _userSession.CurrentUserId && t.Status == "Completed")
                .OrderByDescending(t => t.Update_at)
                .ToListAsync();
            CompletedTasks.Clear();
            foreach (var task in tasks)
                CompletedTasks.Add(task);
        }

        private async void OnRestoreTask(TaskItem task)
        {
            var taskFromDb = await _context.Tasks.FindAsync(task.Id);
            if (task == null) return;
            task.Status = "Active";
            task.Update_at = DateTime.Now;

            await _context.SaveChangesAsync();
            await LoadCompletedTasksAsync();
            RefreshTaskPage();
        }
        private async void OnDeleteCompletedTask(TaskItem task)
        {
            if (task == null) return;
            var result = MessageBox.Show($"Удалить задачу \"{task.Name}\"?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            await LoadCompletedTasksAsync();
        }

        private void RefreshTaskPage()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if(mainWindow?.MainFrame?.Content is TaskPage taskPage)
            {
                _ = taskPage.LoadTasksAsync();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}