using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleProject.Models
{
    public class TaskViewModel
    {
        public string Title { get; set; }
        public DateTime TaskDate { get; set; }

        public int ClientId { get; set; }
        public int ProjectId { get; set; }

        public List<ActivityListModel> Activities { get; set; }
    }
}