using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using TaskForge.Models.Repositories;
using TaskForge.Views;
using TaskForge.Views.Pages;

namespace TaskForge
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Подписываемся на событие навигации Frame
            MainFrame.Navigated += MainFrame_Navigated;
            Loaded += (s, e) => NavigateToProjects();
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // Скрываем кнопку пользователя, если открыта страница пользователя, иначе показываем
            UserBtn.Visibility = e.Content is UserPage ? Visibility.Collapsed : Visibility.Visible;
        }

        private void NavigateToProjects()
        {
            var projectPage = _serviceProvider.GetRequiredService<ProjectPage>();
            MainFrame.Navigate(projectPage);
        }

        private void UserBtn_Click(object sender, RoutedEventArgs e)
        {
            var userPage = _serviceProvider.GetRequiredService<UserPage>();
            MainFrame.Navigate(userPage);
        }

        public void Logout()
        {
            var sessionStorage = _serviceProvider.GetRequiredService<ISessionStorage>();
            sessionStorage.Clear();

            var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();
            authWindow.Show();

            this.Close();
        }
    }
}