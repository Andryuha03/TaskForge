using Microsoft.EntityFrameworkCore;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskForge.Models.Repositories;
using BC = BCrypt.Net.BCrypt;

namespace TaskForge.Views.AuthPages
{
    /// <summary>
    /// Логика взаимодействия для LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        private readonly ApplicationDBContext _dbContext;
        public LogInPage(ApplicationDBContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = textBoxLogIn.Text.Trim();
            string passwordHash = BC.HashPassword(passBox.Password);


            try
            {

                {
                    bool userExists = await _dbContext.Users
                        .AnyAsync(u => u.Name == name && u.Password == passwordHash);


                    if (!userExists)
                    {
                        MessageBox.Show($"Добро пожаловать, {name}",
                            "Успешный вход",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        MainWindow mainWindow = new();
                        mainWindow.Show();
                        Window.GetWindow(this)?.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                {
                    MessageBox.Show("Что-то пошло не так... \n" + ex.Message,
        "Ошибка",
        MessageBoxButton.OK,
        MessageBoxImage.Error);
                }
            }
        }
    }
}
