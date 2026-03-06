using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SampleProject.Models
{
    

    public class UserViewModel
    {

        [Required(ErrorMessage = "Username required")]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z\s]+$",
        ErrorMessage = "Only letters allowed")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Email required")]
        [RegularExpression(
    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
    ErrorMessage = "Invalid email format")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password required")]
        [StringLength(100, MinimumLength = 6)]
        [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
        ErrorMessage = "Password must contain upper, lower, number and special char")]
        public string Password { get; set; }


        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "Select Manager")]
        public int? ManagerId { get; set; }

        [Required(ErrorMessage = "Select Department")]
        public int DepartmentId { get; set; }

    }
}