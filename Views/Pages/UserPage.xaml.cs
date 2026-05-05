using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TaskForge.Models.Repositories;
using TaskForge.ViewModels;
using TaskForge.Views.Dialogs;

namespace TaskForge.Views.Pages
{
    public partial class UserPage : Page
    {
        private readonly IUserSession _userSession;
        private readonly ApplicationDBContext _context;
        private readonly IServiceProvider _serviceProvider;
        private CompletedTasksPage _completedTaskPage;

        public UserPage(IUserSession userSession, ApplicationDBContext context, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _userSession = userSession;
            _context = context;
            _serviceProvider = serviceProvider;
            DataContext = this;

            _userSession.UserChanged += OnUserChanged;

            Loaded += (s, e) => LoadCompletedTaskPage();
        }

        private void OnUserChanged()
        {
            OnPropertyChanged(nameof(CurrentUserName));
            OnPropertyChanged(nameof(CurrentUserEmail));
            OnPropertyChanged(nameof(CurrentUserLevel));
            OnPropertyChanged(nameof(CurrentUserExp));
        }

        public string CurrentUserName => _userSession.CurrentUserName;
        public string CurrentUserEmail => _userSession.CurrentUserEmail;
        public int CurrentUserLevel => _userSession.CurrentUserLevel;
        public int CurrentUserExp => _userSession.CurrentUserTotalEx;

        private void LoadCompletedTaskPage()
        {
            _completedTaskPage = _serviceProvider.GetRequiredService<CompletedTasksPage>();
            CompletedTasksFrame.Navigate(_completedTaskPage);
        }

        public void RefreshCompletedTask()
        {
            _completedTaskPage?.LoadCompletedTasksAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)=>PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new EditProfileViewModel(_userSession, _context);
            var window = new EditProfileWindow(viewModel);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.Logout();
        }
    }
}