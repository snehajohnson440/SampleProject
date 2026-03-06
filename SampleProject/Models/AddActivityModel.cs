using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleProject.Models
{
    public class AddActivityModel
    {
        public int TaskId { get; set; }

        public int ActivityTypeId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public decimal Hours { get; set; }
    }
}