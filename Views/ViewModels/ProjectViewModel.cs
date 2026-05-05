using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using TaskForge.Models.Entities;

namespace TaskForge.Views.ViewModels
{
    public class ProjectDisplay : INotifyPropertyChanged
    {
        public Project Project { get; set; }
        public ObservableCollection<TaskItem> Tasks { get; set; }
        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }
        public SolidColorBrush ColorBrush => new SolidColorBrush((Color)ColorConverter.ConvertFromString(Project.Color_mark));
        public SolidColorBrush ColorLightBrush
        {
            get
            {
                var color = (Color)ColorConverter.ConvertFromString(Project.Color_mark);
                var light = Color.FromArgb(80, color.R, color.G, color.B);
                return new SolidColorBrush(light);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
