using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeProjectManagement.Models.ViewModels.Manager
{
    public class ProjectEmployeeDetailsViewModel
    {
        public string ProjectName { get; set; }
        public double Cost { get; set; }

        public int? ProjectId { get; set; }
        public int? EmployeeId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JoiningDate { get; set; }
    }
}