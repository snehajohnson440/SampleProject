using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleProject.Models
{
    public class TaskListModel
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public DateTime TaskDate { get; set; }
        public string Status { get; set; }

        public int AssignedTo { get; set; }

        public string ClientName { get; set; }
        public string ProjectName { get; set; }

        public decimal TotalHours { get; set; }
    }
}