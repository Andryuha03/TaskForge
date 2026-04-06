using System;
using System.Collections.Generic;
using System.Text;

namespace TaskForge.Models.Entities
{
    public class User_Achievement
    {
        public int User_id {  get; set; }
        public int Achievement_id { get; set; }
        public DateTime Earned_at { get; set; }

        public Achievement Achievement { get; set; }
        public User User { get; set; }
    }
}
