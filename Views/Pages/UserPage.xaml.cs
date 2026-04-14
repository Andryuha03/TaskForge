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

namespace TaskForge.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private readonly IUserSession _userSession;
        public UserPage(IUserSession userSession)
        {
            InitializeComponent();
            _userSession = userSession;

            this.DataContext = _userSession;
        }

        private void OpenAllAchievements_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
