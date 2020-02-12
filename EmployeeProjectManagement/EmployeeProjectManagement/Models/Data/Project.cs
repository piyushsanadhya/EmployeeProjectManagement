using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployeeProjectManagement.Models.Data
{
    public class Project : AuditableEntity
    {
        public string Name { get; set; }

        public double Cost { get; set; }
    }
}