using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EmployeeProjectManagement.Models.Data
{
    public class AuditableEntity : IAuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedOnUtc { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime? LastModifiedOnUtc { get; set; }

        public string LastModifiedBy { get; set; }
    }
}