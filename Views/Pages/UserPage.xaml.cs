using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using TaskForge.Models.Repositories;

namespace TaskForge.Views.Pages
{
    public partial class UserPage : Page
    {
        private readonly IUserSession _userSession;
        private readonly ApplicationDBContext _context;
        private readonly IServiceProvider _serviceProvider;
        private CompletedTasksPage _completedTaskPage;

        public UserPage(IUserSession userSession, ApplicationDBContext context, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _userSession = userSession;
            _context = context;
            _serviceProvider = serviceProvider;
            DataContext = this;

            Loaded += (s, e) => LoadCompletedTaskPage();
        }

        private void LoadCompletedTaskPage()
        {
            _completedTaskPage = _serviceProvider.GetRequiredService<CompletedTasksPage>();
            CompletedTasksFrame.Navigate(_completedTaskPage);
        }

        public void RefreshCompletedTask()
        {
            _completedTaskPage?.LoadCompletedTasksAsync();
        }

        public string CurrentUserName => _userSession.CurrentUserName;
        public string CurrentUserEmail => _userSession.CurrentUserEmail;
        public int CurrentUserLevel => _userSession.CurrentUserLevel;
        public int CurrentUserExp => _userSession.CurrentUserTotalEx;

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            // Открыть диалог редактирования профиля
            // Можно реализовать отдельно
        }
    }
}