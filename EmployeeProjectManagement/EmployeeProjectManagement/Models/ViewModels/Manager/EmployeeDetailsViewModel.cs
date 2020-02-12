using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeProjectManagement.Models.ViewModels.Manager
{
    public class EmployeeDetailsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JoiningDate { get; set; }
        public int ProjectId { get; set; }
        public int? EmployeeId { get; set; }
    }
}