using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskForge.Models.Entities
{
    public partial class TaskItem : INotifyPropertyChanged
    {

        private int _actual_time;
        public int Id { get; set; }
        public int User_id { get; set; }
        public int? Project_id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public int Priority { get; set; }
        public int Actual_time
        {
            get => _actual_time;
            set
            {
                if (_actual_time != value)
                {
                    _actual_time = value;
                    OnPropertyChanged(nameof(Actual_time));
                    OnPropertyChanged(nameof(ActualTimeDisplay));
                }
            }
        }
        public string Status { get; set; }
        public int Reward_exp { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Update_at { get; set; }
        public bool Is_rewarded { get; set; } = false;
        public int Planned_time_seconds { get; set; } = 86400;




        public User User { get; set; }
        public Project Project { get; set; }


        public string ActualTimeDisplay => TimeSpan.FromSeconds(Actual_time).ToString(@"hh\:mm\:ss");
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
