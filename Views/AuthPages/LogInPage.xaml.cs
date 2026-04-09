using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;
using BC = BCrypt.Net.BCrypt;

namespace TaskForge.Views.AuthPages
{
    public partial class LogInPage : Page
    {
        private readonly MainWindow _mainWindow;
        private readonly IUserSession _userSession;
        private readonly IUserRepository _userRepository;
        public LogInPage(IUserRepository userRepository,
            IUserSession userSession,
            MainWindow mainWindow)
        {
            InitializeComponent();
            _userRepository = userRepository;
            _mainWindow = mainWindow;
            _userSession = userSession;
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = textBoxLogIn.Text.Trim();
            string password = passBox.Password;

            try
            {

                {
                    User? user = await _userRepository.GetUserAsync(name);

                    if (user == null || !BC.Verify(password, user.Password))
                    {
                        MessageBox.Show($"Неверное имя или пароль",
                        "Ошибка входа",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                        return;
                    }

                    _userSession.SetCurrentUser(user);

                    MessageBox.Show($"Добро пожаловать, {name}",
                    "Успешный вход",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                    
                    _mainWindow.Show();
                    Window.GetWindow(this).Close();
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
