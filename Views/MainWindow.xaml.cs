using Microsoft.Extensions.DependencyInjection;
using System;
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
        private readonly IUserSession _userSession;
        private readonly ApplicationDBContext _dbContext;
        private readonly Func<AuthWindow> _authWindowFactory;

        public MainWindow(IUserSession userSession, ApplicationDBContext dbContext, Func<AuthWindow> authWindowFactory)
        {
            InitializeComponent();
            _userSession = userSession;
            _dbContext = dbContext;
            _authWindowFactory = authWindowFactory;
            Loaded += (s, e) => NavigateToProjects();
        }

        private void NavigateToProjects()
        {
            var projectPage = App.serviceProvider.GetRequiredService<ProjectPage>();
            MainFrame.Navigate(projectPage);
        }

        // Кнопка пользователя (круглая)
        private void UserBtn_Click(object sender, RoutedEventArgs e)
        {
            var userPage = App.serviceProvider.GetRequiredService<UserPage>();
            MainFrame.Navigate(userPage);
        }

        // Метод для выхода (вызывается из UserPage)
        public void Logout()
        {
            var sessionStorage = App.serviceProvider.GetRequiredService<ISessionStorage>();
            sessionStorage.Clear();

            var authWindow = App.serviceProvider.GetRequiredService<AuthWindow>();
            authWindow.Show();

            this.Close();
        }
    }
}