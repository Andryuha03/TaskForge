using System;
using System.Collections.Generic;
using System.Text;

namespace TaskForge.Models
{
    public class User
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public string Email {get;set;}
        public string Password {get;set;}
        public int Level {get;set;}
        public int Total_exp {get;set;}
        public DateTime Created_at {get;set;}
        public DateTime Updated_at {get;set;}
    }
}
