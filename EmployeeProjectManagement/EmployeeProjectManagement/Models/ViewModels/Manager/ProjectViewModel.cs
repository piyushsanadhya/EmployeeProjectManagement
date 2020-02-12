using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployeeProjectManagement.Models.ViewModels.Manager
{
    public class ProjectViewModel
    {
        public int? Id { get; set; }

        [StringLength(200,ErrorMessage ="Max length is reached.")]
        [Required(ErrorMessage ="This field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public double Cost { get; set; }
    }
}