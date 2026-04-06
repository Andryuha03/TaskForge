using System.Collections.ObjectModel;
using TaskForge.Models.Repositories;
using TaskForge.Models.Entities;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace TaskForge.Views
{
    internal class MainViewModel
    {
        private ApplicationDBContext _dbContext;

        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Project> Projects { get; set; }
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public ICommand LoadDataCommand { get; set; }
        public ICommand AddUserCommand { get; set; }

    }
}
