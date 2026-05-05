using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using TaskForge.Models.Entities;

namespace TaskForge.ViewModels
{
    public class ProjectDisplay : INotifyPropertyChanged
    {
        private Project _project;
        private bool _isExpanded;
        private ObservableCollection<TaskItem> _tasks;

        public Project Project
        {
            get => _project;
            set { _project = value; OnPropertyChanged(); OnPropertyChanged(nameof(Name)); OnPropertyChanged(nameof(Info)); OnPropertyChanged(nameof(ColorBrush)); }
        }

        public string Name => Project?.Name ?? "Без названия";
        public string Info => Project?.Info ?? "";

        public SolidColorBrush ColorBrush
        {
            get
            {
                if (Project == null || string.IsNullOrEmpty(Project.Color_mark))
                    return new SolidColorBrush(Colors.LightGray);
                try
                {
                    var color = (Color)ColorConverter.ConvertFromString(Project.Color_mark);
                    return new SolidColorBrush(color);
                }
                catch
                {
                    return new SolidColorBrush(Colors.LightGray);
                }
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TaskItem> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}