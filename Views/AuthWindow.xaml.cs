using Microsoft.Extensions.DependencyInjection;
using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskForge.Models.Repositories;
using TaskForge.Views.AuthPages;

namespace TaskForge.Views
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        public AuthWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            NavigateToLogin();
        }

        private void LogInBtn_Click(object sender, RoutedEventArgs e) => NavigateToLogin();
        private void SignUpBtn_Click(object sender, RoutedEventArgs e) => NavigateToSignUp();

        private void NavigateToLogin()
        {
            AuthFrame.Navigate(_serviceProvider.GetRequiredService<LogInPage>());
            SignUpBtn.Visibility = Visibility.Visible;
            LogInBtn.Visibility = Visibility.Collapsed;
        }

        private void NavigateToSignUp()
        {
            AuthFrame.Navigate(_serviceProvider.GetRequiredService<SignUpPage>());
            SignUpBtn.Visibility = Visibility.Collapsed;
            LogInBtn.Visibility = Visibility.Visible;
        }

        public void OnSuccessfulLogin()
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            this.Close();
        }
    }
}
