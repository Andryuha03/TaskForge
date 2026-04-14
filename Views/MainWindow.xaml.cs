using System.Windows;
using TaskForge.Models.Repositories;
using TaskForge.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using MaterialDesignThemes.Wpf;

namespace TaskForge
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserSession _userSession;
        private readonly ApplicationDBContext _dbContext;
        public MainWindow(IUserSession userSession, ApplicationDBContext dbContext)
        {
            InitializeComponent();
            _userSession = userSession;
            _dbContext = dbContext;
            this.Visibility = Visibility.Hidden;
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UserPage(_userSession));
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
    }
}