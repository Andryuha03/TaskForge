using System;
using System.Collections.Generic;
using System.Text;

namespace TaskForge.Models.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string Color_mark { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Update_at { get; set; }

        public User User { get; set; }

        public ICollection<TaskItem> Tasks { get; set; }
    }
}

