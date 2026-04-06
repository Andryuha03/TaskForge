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

namespace TaskForge.Views
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();

            AuthFrame.Navigate(new AuthPages.LogInPage());
            LogInBtn.Visibility = Visibility.Collapsed;

        }

        private void LogInBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthFrame.Navigate(new AuthPages.LogInPage());

            LogInBtn.Visibility = Visibility.Collapsed;
            SignUpBtn.Visibility = Visibility.Visible;
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthFrame.Navigate(new AuthPages.SignUpPage());

            SignUpBtn.Visibility = Visibility.Collapsed;
            LogInBtn.Visibility = Visibility.Visible;

        }
    }
}
