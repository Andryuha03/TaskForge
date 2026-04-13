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
    /// Логика взаимодействия для TaskPage.xaml
    /// </summary>
    public partial class TaskPage : Page
    {
        private readonly ApplicationDBContext _dbContext;
        public TaskPage(ApplicationDBContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;

            this.DataContext = _dbContext;
        }

    }
}
