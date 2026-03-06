using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleProject.Models
{
    public class PerformanceModelInMVC
    {
        public int UserId { get; set; }
        public int ManagerId { get; set; }

        public decimal TaskCompletion { get; set; }
        public decimal Productivity { get; set; }
        public decimal Consistency { get; set; }
        public decimal QualityOfWork { get; set; }
        public decimal Communication { get; set; }
        public decimal Teamwork { get; set; }
        public decimal Punctuality { get; set; }
        public decimal ProblemSolving { get; set; }
        public decimal Initiative { get; set; }
        public decimal LearningAbility { get; set; }

        public decimal AverageScore { get; set; }

        public string Feedback { get; set; }
    }
}
