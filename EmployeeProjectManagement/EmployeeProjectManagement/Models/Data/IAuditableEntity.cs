using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeProjectManagement.Models.Data
{
    interface IAuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        int Id { get; set; }

        [Required]
        DateTime CreatedOnUtc { get; set; }

        [Required]
        string CreatedBy { get; set; }

        DateTime? LastModifiedOnUtc { get; set; }

        string LastModifiedBy { get; set; }
    }
}
