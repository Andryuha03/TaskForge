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
        private readonly ApplicationDBContext _dbContext;

        public AuthWindow()
        {
            InitializeComponent();
            _dbContext = new ApplicationDBContext();

            AuthFrame.Navigate(new LogInPage(_dbContext));
            LogInBtn.Visibility = Visibility.Collapsed;

        }

        private void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthFrame.Navigate(new LogInPage(_dbContext));

            LogInBtn.Visibility = Visibility.Collapsed;
            SignUpBtn.Visibility = Visibility.Visible;
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthFrame.Navigate(new SignUpPage(_dbContext));

            SignUpBtn.Visibility = Visibility.Collapsed;
            LogInBtn.Visibility = Visibility.Visible;

        }
        protected override void OnClosed(EventArgs e)
        {
            _dbContext?.Dispose();
            base.OnClosed(e);
        }
    }
}
