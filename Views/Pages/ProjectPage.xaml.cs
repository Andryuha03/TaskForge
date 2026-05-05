using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using TaskForge.Helpers;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;
using TaskForge.ViewModels;
using TaskForge.Views.Dialogs;

namespace TaskForge.Views.Pages
{
    public partial class ProjectPage : Page, INotifyPropertyChanged
    {
        private readonly ApplicationDBContext _context;
        private readonly IUserSession _userSession;
        private ObservableCollection<ProjectDisplay> _projects;

        public ObservableCollection<ProjectDisplay> Projects
        {
            get => _projects;
            set { _projects = value; OnPropertyChanged(nameof(Projects)); }
        }

        public ICommand AddProjectCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand CompleteTaskCommand { get; }

        public ProjectPage(ApplicationDBContext context, IUserSession userSession)
        {
            InitializeComponent();
            _context = context;
            _userSession = userSession;
            Projects = new ObservableCollection<ProjectDisplay>();
            DataContext = this;

            AddProjectCommand = new RelayCommand(OnAddProject);
            DeleteProjectCommand = new RelayCommand<ProjectDisplay>(OnDeleteProject);
            AddTaskCommand = new RelayCommand<ProjectDisplay>(OnAddTask);
            CompleteTaskCommand = new RelayCommand<TaskItem>(OnCompleteTask);

            Loaded += async (s, e) => await LoadProjectsAsync();
        }

        private async Task LoadProjectsAsync()
        {
            var projects = await _context.Projects
                .Where(p => p.User_id == _userSession.CurrentUserId && p.Status != "Completed")
                .Include(p => p.Tasks)
                .OrderBy(p => p.Name)
                .ToListAsync();

            Projects.Clear();
            foreach (var proj in projects)
            {
                var display = new ProjectDisplay
                {
                    Project = proj,
                    Tasks = new ObservableCollection<TaskItem>(
                        proj.Tasks.OrderByDescending(t => t.Priority).ThenBy(t => t.Created_at)
                    ),
                    IsExpanded = false
                };
                Projects.Add(display);
            }
        }

        private void OnAddProject()
        {
            var vm = new ProjectEditViewModel(_context, _userSession.CurrentUserId);
            var window = new ProjectEditWindow(vm);
            window.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            if (window.ShowDialog() == true)
                _ = LoadProjectsAsync();
        }

        private async void OnDeleteProject(ProjectDisplay display)
        {
            if (display?.Project == null) return;
            var result = MessageBox.Show($"Удалить проект \"{display.Name}\"?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            _context.Projects.Remove(display.Project);
            await _context.SaveChangesAsync();
            await LoadProjectsAsync();
            RefreshUserPageCompleted();
        }

        private void OnAddTask(ProjectDisplay display)
        {
            var vm = new TaskEditViewModel(_context, _userSession.CurrentUserId);
            vm.ProjectId = display.Project.Id;
            var window = new TaskEditWindow(vm);
            window.Owner = Window.GetWindow(this);
            if (window.ShowDialog() == true)
                _ = LoadProjectsAsync();
        }

        private async void OnCompleteTask(TaskItem task)
        {
            if (task == null || task.Status == "Completed") return;
            task.Status = "Completed";
            task.Update_at = DateTime.Now;
            await _context.SaveChangesAsync();

            // Проверяем, все ли задачи проекта завершены
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == task.Project_id);
            if (project != null && project.Tasks.All(t => t.Status == "Completed"))
            {
                project.Status = "Completed";
                project.Update_at = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            await LoadProjectsAsync();
            RefreshUserPageCompleted();
        }

        private void RefreshUserPageCompleted()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow?.MainFrame?.Content is UserPage userPage)
                userPage.RefreshCompletedProjects();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}