using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;
using BC = BCrypt.Net.BCrypt;

namespace TaskForge.Views.AuthPages
{
    public partial class SignUpPage : Page
    {
        private readonly ApplicationDBContext _dbContext;
        private bool isNameValid = false;
        private bool isPasswordValid = false;
        private bool isEmailValid = false;

        public SignUpPage(ApplicationDBContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
        }

        private async void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = textBoxLogIn.Text.Trim();
            string hashPassword = BC.HashPassword(passBox.Password);
            string email = textBoxEmail.Text.Trim();
            try
            {

                bool userExists = await _dbContext.Users
                    .AnyAsync(u => u.Name == name);
                if (userExists)
                {
                    ValidationSnackbar.MessageQueue?.Enqueue("Это имя уже занято");
                    return;
                }

                bool emailExists = await _dbContext.Users
                    .AnyAsync(u => u.Email == email);
                if (emailExists)
                {
                    ValidationSnackbar.MessageQueue?.Enqueue("Эта почта уже используется");
                    return;
                }

                var newUser = new User
                {
                    Name = name,
                    Password = hashPassword,
                    Email = email,
                };

                _dbContext .Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                MessageBox.Show("Вы успешно зарегистрировались!",
                    "Успешный вход!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                MainWindow mainWindow = new();
                mainWindow.Show();
                Window.GetWindow(this).Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так... \n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void TextBoxLogIn_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = textBoxLogIn.Text.Trim();
            isNameValid = !string.IsNullOrEmpty(name);
            if (!isNameValid)
                textBoxLogIn.BorderBrush = Brushes.Red;
            else
            {
                textBoxLogIn.BorderBrush = Brushes.Green;
                UpdateSaveButtonState();
            }
        }
        private void PassBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = passBox.Password;
            isPasswordValid = !string.IsNullOrWhiteSpace(password) && password.Length >= 5;

            if (!isPasswordValid)
            {
                passBox.BorderBrush = Brushes.Red;
                passBox.ToolTip = "Минимум 5 символов";
            }
            else
            {
                passBox.BorderBrush = Brushes.Green;
                UpdateSaveButtonState();
            }

        }

        private void TextBoxEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            string email = textBoxEmail.Text.Trim().ToLower();
            isEmailValid = !string.IsNullOrEmpty(textBoxEmail.Text)
                && email.Contains('@')
                && !email.StartsWith('@')
                && !email.EndsWith('@');
            if (!isEmailValid)
                textBoxEmail.BorderBrush = Brushes.Red;
            else
            {
                textBoxEmail.BorderBrush = Brushes.Green;
                UpdateSaveButtonState();
            }
        }

        private void UpdateSaveButtonState()
        {
            bool allValid = isNameValid && isPasswordValid && isEmailValid;
            SignUpBtn.IsEnabled = allValid;
        }
    }

}
