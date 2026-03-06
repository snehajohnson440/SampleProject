using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleProject.Models
{
    public class ActivityListModel
    {
        public int ActivityId { get; set; }
        public int ActivityTypeId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public decimal ActivityHours { get; set; }
        public DateTime ActivityDate { get; set; }

        public string ActivityTypeName { get; set; }
    }
}