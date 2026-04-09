using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskForge.Models.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public int? Project_id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public int Priority { get; set; }
        public int Actual_time { get; set; }
        public string Status { get; set; }
        public int Reward_exp { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Update_at { get; set; }

        public User User { get; set; }
        public Project Project { get; set; }
    }
}
