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
using TaskForge.Views.Pages;

namespace TaskForge
{
    public partial class MainWindow : Window
    {
        private readonly IUserSession _userSession;
        private readonly ApplicationDBContext _dbContext;
        public MainWindow(IUserSession userSession, ApplicationDBContext dbContext)
        {
            InitializeComponent();
            _userSession = userSession;
            _dbContext = dbContext;
            this.Visibility = Visibility.Hidden;
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UserPage(_userSession));
        }

        private void TaskBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TaskPage(_dbContext));
        }

        private void ProjectBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProjectPage());
        }

        public void ShowMainContent()
        {
            this.Visibility = Visibility.Visible;
        }
    }
}