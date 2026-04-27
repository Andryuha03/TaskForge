using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;

namespace TaskForge.Views.Pages
{
    public partial class CompletedTasksPage : Page, INotifyPropertyChanged
    {
        private readonly ApplicationDBContext _context;
        private readonly IUserSession _userSession;
        private ObservableCollection<TaskItem> _completedTasks;

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}