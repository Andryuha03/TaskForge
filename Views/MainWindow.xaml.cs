using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TaskForge.Models.Repositories;
using TaskForge.Views;
using TaskForge.Views.Pages;

namespace TaskForge
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserSession _userSession;
        private readonly ApplicationDBContext _dbContext;
        private readonly Func<AuthWindow> _authWindowFactory;
        public MainWindow(IUserSession userSession, ApplicationDBContext dbContext, Func<AuthWindow> authWindowFactory)
        {
            InitializeComponent();
            _userSession = userSession;
            _dbContext = dbContext;
            _authWindowFactory = authWindowFactory;

        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            var userPage = App.serviceProvider.GetRequiredService<UserPage>();
            MainFrame.Navigate(userPage);
        }

        private void TaskBtn_Click(object sender, RoutedEventArgs e)
        {
            var taskPage = App.serviceProvider.GetRequiredService<TaskPage>();
            MainFrame.Navigate(taskPage);
        }

        private void ProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProjectPage());
        }

        public DialogHost MainDialogHost => RootDialogHost;

        public void ShowMainContent()
        {
            this.Visibility = Visibility.Visible;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var sessionStorage = App.serviceProvider.GetRequiredService<ISessionStorage>();
            sessionStorage?.Clear();

            var authWindow = _authWindowFactory();
            authWindow.Show();
            this.Hide();
        }
    }
}