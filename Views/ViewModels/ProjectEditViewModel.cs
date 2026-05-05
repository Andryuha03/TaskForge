using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using TaskForge.Helpers;
using TaskForge.Models.Entities;
using TaskForge.Models.Repositories;

namespace TaskForge.ViewModels
{
    public class ProjectEditViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDBContext _context;
        private readonly int _currentUserId;
        private readonly Project _editingProject;
        private readonly bool _isNew;

        // Поля для свойств
        private string _name;
        private string _info = string.Empty;      // не null
        private DateTime _deadline;
        private string _colorMark = "#FFFFFF";   // не null
        private Color _selectedColor = Colors.White;

        // Свойства для привязки
        public string Name
        {
            get => _name;
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

        public string Info
        {
            get => _info;
            set
            {
                if (_info != value)
                {
                    _info = value ?? string.Empty;
                    OnPropertyChanged(nameof(Info));
                }
            }
        }

        public DateTime Deadline
        {
            get => _deadline;
            set
            {
                if (_deadline != value)
                {
                    _deadline = value;
                    OnPropertyChanged(nameof(Deadline));
                }
            }
        }

        public string ColorMark
        {
            get => _colorMark;
            set
            {
                if (_colorMark != value)
                {
                    _colorMark = value ?? "#FFFFFF";
                    OnPropertyChanged(nameof(ColorMark));
                    // Обновляем SelectedColor, если нужно
                    if (!string.IsNullOrEmpty(_colorMark))
                    {
                        try
                        {
                            SelectedColor = (Color)ColorConverter.ConvertFromString(_colorMark);
                        }
                        catch { /* оставляем текущий цвет */ }
                    }
                }
            }
        }

        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    OnPropertyChanged(nameof(SelectedColor));
                    // Преобразуем обратно в HEX
                    ColorMark = $"#{value.R:X2}{value.G:X2}{value.B:X2}";
                }
            }
        }

        public bool IsNew => _isNew;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<bool> RequestClose;
        public event PropertyChangedEventHandler PropertyChanged;

        public ProjectEditViewModel(ApplicationDBContext context, int currentUserId, Project projectToEdit = null)
        {
            _context = context;
            _currentUserId = currentUserId;
            _editingProject = projectToEdit;
            _isNew = projectToEdit == null;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => RequestClose?.Invoke(this, false));

            if (!_isNew)
            {
                Name = _editingProject.Name ?? "";
                Info = _editingProject.Info ?? "";
                Deadline = _editingProject.Deadline;
                ColorMark = _editingProject.Color_mark ?? "#FFFFFF";
            }
            else
            {
                Deadline = DateTime.Now.AddDays(7);
                ColorMark = "#FFFFFF";
            }
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(Name);

        private async void Save()
        {
            if (!CanSave()) return;

            if (string.IsNullOrEmpty(Info)) Info = "";
            if (string.IsNullOrEmpty(ColorMark)) ColorMark = "#FFFFFF";

            if (_isNew)
            {
                var project = new Project
                {
                    User_id = _currentUserId,
                    Name = Name,
                    Info = Info,
                    Deadline = Deadline,
                    Color_mark = ColorMark,
                    Status = "Active",
                    Created_at = DateTime.Now,
                    Update_at = DateTime.Now
                };
                _context.Projects.Add(project);
            }
            else
            {
                _editingProject.Name = Name;
                _editingProject.Info = Info;
                _editingProject.Deadline = Deadline;
                _editingProject.Color_mark = ColorMark;
                _editingProject.Update_at = DateTime.Now;
                if (string.IsNullOrEmpty(_editingProject.Status))
                    _editingProject.Status = "Active";
            }

            await _context.SaveChangesAsync();
            RequestClose?.Invoke(this, true);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}