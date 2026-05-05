using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using TaskForge.Helpers;
using TaskForge.Models.Repositories;
using TaskForge.ViewModels;
using TaskForge.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using TaskForge.Models.Entities;

namespace TaskForge.Views.Pages
{
    public partial class UserPage : Page, INotifyPropertyChanged
    {
        private readonly IUserSession _userSession;
        private readonly ApplicationDBContext _context;
        private readonly IServiceProvider _serviceProvider;
        private ObservableCollection<ProjectDisplay> _completedProjects;

        public ObservableCollection<ProjectDisplay> CompletedProjects
        {
            get => _completedProjects;
            set { _completedProjects = value; OnPropertyChanged(nameof(CompletedProjects)); }
        }

        public ICommand GoBackCommand { get; }

        public UserPage(IUserSession userSession, ApplicationDBContext context, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _userSession = userSession;
            _context = context;
            _serviceProvider = serviceProvider;
            CompletedProjects = new ObservableCollection<ProjectDisplay>();
            DataContext = this;

            GoBackCommand = new RelayCommand(OnGoBack);
            Loaded += async (s, e) => await LoadCompletedProjectsAsync();
        }

        public string CurrentUserName => _userSession.CurrentUserName;
        public string CurrentUserEmail => _userSession.CurrentUserEmail;
        public int CurrentUserLevel => _userSession.CurrentUserLevel;
        public int CurrentUserExp => _userSession.CurrentUserTotalEx;

        public async Task LoadCompletedProjectsAsync()
        {
            var projects = await _context.Projects
                .Where(p => p.User_id == _userSession.CurrentUserId && p.Status == "Completed")
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.Update_at)
                .ToListAsync();

            CompletedProjects.Clear();
            foreach (var proj in projects)
            {
                var display = new ProjectDisplay
                {
                    Project = proj,
                    Tasks = new ObservableCollection<TaskItem>(proj.Tasks.ToList()),
                    IsExpanded = false
                };
                CompletedProjects.Add(display);
            }
        }

        public async void RefreshCompletedProjects()
        {
            await LoadCompletedProjectsAsync();
        }

        private void OnGoBack()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                var projectPage = _serviceProvider.GetRequiredService<ProjectPage>();
                mainWindow.MainFrame.Navigate(projectPage);
            }
        }

        private async void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            var vm = new EditProfileViewModel(_userSession, _context);
            var window = new EditProfileWindow(vm);
            window.Owner = Window.GetWindow(this);
            if (window.ShowDialog() == true)
            {
                OnPropertyChanged(nameof(CurrentUserName));
                OnPropertyChanged(nameof(CurrentUserEmail));
                OnPropertyChanged(nameof(CurrentUserLevel));
                OnPropertyChanged(nameof(CurrentUserExp));
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.Logout();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}