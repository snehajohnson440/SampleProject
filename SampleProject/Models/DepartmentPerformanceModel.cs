using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleProject.Models
{
    public class DepartmentPerformanceModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalReviews { get; set; }
        public decimal DepartmentAverageScore { get; set; }
    }
}