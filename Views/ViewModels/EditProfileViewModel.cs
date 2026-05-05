using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BC = BCrypt.Net.BCrypt;
using TaskForge.Models.Repositories;
using TaskForge.Views.Dialogs;
using TaskForge.Helpers;

namespace TaskForge.ViewModels
{
    public class EditProfileViewModel : INotifyPropertyChanged
    {
        private readonly IUserSession _userSession;
        private readonly ApplicationDBContext _context;

        private string _name;
        private string _email;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<bool> RequestClose;
        public event PropertyChangedEventHandler PropertyChanged;

        public EditProfileViewModel(IUserSession userSession, ApplicationDBContext context)
        {
            _userSession = userSession;
            _context = context;
            Name = userSession.CurrentUserName;
            Email = userSession.CurrentUserEmail;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(() => RequestClose?.Invoke(this, false));
        }

        private async void Save()
        {
            var user = await _context.Users.FindAsync(_userSession.CurrentUserId);
            if (user == null) return;

            user.Name = Name;
            user.Email = Email;

            var window = Application.Current.Windows.OfType<EditProfileWindow>().FirstOrDefault();
            var newPassword = window?.PasswordBox.Password;
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                user.Password = BC.HashPassword(newPassword);
            }

            await _context.SaveChangesAsync();

            _userSession.CurrentUserName = Name;
            _userSession.CurrentUserEmail = Email;
            _userSession.OnUserChanged();

            RequestClose?.Invoke(this, true);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}