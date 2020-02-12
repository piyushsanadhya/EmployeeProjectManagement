using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EmployeeProjectManagement.Models.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EmployeeProjectManagement.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<EmployeeToProject> EmployeeToProjects { get; set; }


        public int SaveChanges(string username)
        {
            AddAuditInfo(username);

            return SaveChanges();
        }

        public async Task<int> SaveChangesAsync(string username, CancellationToken cancellationToken = default(CancellationToken))
        {
            AddAuditInfo(username);

            return await SaveChangesAsync(cancellationToken);
        }

        private void AddAuditInfo(string username)
        {
            var addedAuditedEntities = ChangeTracker.Entries<IAuditableEntity>()
                                                    .Where(p => p.State == EntityState.Added)
                                                    .Select(p => p.Entity);

            var modifiedAuditedEntities = ChangeTracker.Entries<IAuditableEntity>()
                                                       .Where(p => p.State == EntityState.Modified)
                                                       .Select(p => p.Entity);

            var utcNow = DateTime.UtcNow;

            foreach (var added in addedAuditedEntities)
            {
                added.CreatedOnUtc = utcNow;
                added.CreatedBy = username;
            }

            foreach (var modified in modifiedAuditedEntities)
            {
                modified.LastModifiedOnUtc = utcNow;
                modified.LastModifiedBy = username;
            }
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}