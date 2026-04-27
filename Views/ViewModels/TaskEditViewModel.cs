using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;
using TaskForge.Views.Pages;
using TaskForge.Helpers;

namespace TaskForge.ViewModels
{
    public class TaskEditViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDBContext _context;
        private readonly int _currentUserId;
        private TaskItem _editingTask;
        private bool _isNew;
        private string _name;
        public string Name
        {
            get=> _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }
        public string Info { get; set; }
        public int Priority { get; set; } = 1;
        public int RewardExp { get; set; } = 10;
        public int? ProjectId { get; set; }

        public ObservableCollection<int> Priorities { get; } = new ObservableCollection<int> { 1, 2, 3, 4, 5 };
        public ObservableCollection<Project> ProjectsList { get; set; }

        public bool IsNew => _isNew;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<bool> RequestClose;




        public TaskEditViewModel(ApplicationDBContext context, int currentUserId, TaskItem taskToEdit = null)
        {
            _context = context;
            _currentUserId = currentUserId;
            _editingTask = taskToEdit;
            _isNew = taskToEdit == null;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => RequestClose?.Invoke(this, false));

            LoadProjects();

            if (!_isNew)
            {
                Name = _editingTask.Name;
                Info = _editingTask.Info;
                Priority = _editingTask.Priority;
                RewardExp = _editingTask.Reward_exp;
                ProjectId = _editingTask.Project_id;
            }
        }

        private async void LoadProjects()
        {
            var projects = await _context.Projects
                .Where(p => p.User_id == _currentUserId)
                .ToListAsync();
            ProjectsList = new ObservableCollection<Project>(projects);
            OnPropertyChanged(nameof(ProjectsList));
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(Name);

        private async void Save()
        {
            if (!CanSave()) return;

            if (_isNew)
            {
                var task = new TaskItem
                {
                    User_id = _currentUserId,
                    Name = Name,
                    Info = Info,
                    Priority = Priority,
                    Reward_exp = RewardExp,
                    Project_id = ProjectId,
                    Status = "Active",
                    Created_at = DateTime.Now,
                    Update_at = DateTime.Now
                };
                _context.Tasks.Add(task);
            }
            else
            {
                _editingTask.Name = Name;
                _editingTask.Info = Info;
                _editingTask.Priority = Priority;
                _editingTask.Reward_exp = RewardExp;
                _editingTask.Project_id = ProjectId;
                _editingTask.Update_at = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            RequestClose?.Invoke(this, true);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}