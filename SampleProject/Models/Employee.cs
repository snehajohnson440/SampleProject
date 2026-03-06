using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleProject.Models
{
    public class Employee
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public int? ManagerId { get; set; }

        public string Role { get; set; }
    }
}