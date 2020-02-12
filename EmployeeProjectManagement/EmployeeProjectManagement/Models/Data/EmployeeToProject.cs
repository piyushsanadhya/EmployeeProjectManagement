using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeProjectManagement.Models.Data
{
    public class EmployeeToProject : AuditableEntity
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }

        public virtual Project Project { get; set; }
        public virtual Employee Employee { get; set; }
    }
}