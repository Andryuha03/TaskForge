using System;
using System.Collections.Generic;
using System.Text;

namespace TaskForge.Models.Entities
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }

        public ICollection<User_Achievement> Users_Achievements { get; set; }
    }
}
