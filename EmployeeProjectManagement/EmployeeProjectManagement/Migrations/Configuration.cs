namespace EmployeeProjectManagement.Migrations
{
    using EmployeeProjectManagement.Enum;
    using EmployeeProjectManagement.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EmployeeProjectManagement.Models.ApplicationDbContext>
    {
        private const string _defaultAdminUserName = "appadmin@example.com";
        private const string _defaultAdminPassword = "Password@123";
        private const string _defaultAdminEmail = "appadmin@example.com";

        private const string _defaultManagerUserName = "manager@example.com";
        private const string _defaultManagerPassword = "Password@123";
        private const string _defaultManagerEmail = "manager@example.com";

        private string _adminRole = RolesEnum.SuperAdmin.ToString();
        private string _employeeRole = RolesEnum.Employee.ToString();
        private string _managerRole = RolesEnum.Manager.ToString();

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(EmployeeProjectManagement.Models.ApplicationDbContext context)
        {
            if (!context.Roles.Any(r => r.Name == _adminRole))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = _adminRole };

                manager.Create(role);
            }

            if (!context.Users.Any(u => u.UserName == _defaultAdminEmail))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = _defaultAdminEmail };

                manager.Create(user, _defaultAdminPassword);
                manager.AddToRole(user.Id, _adminRole);
            }

            if (!context.Roles.Any(r => r.Name == _managerRole))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = _managerRole };

                manager.Create(role);
            }

            if (!context.Users.Any(u => u.UserName == _defaultManagerEmail))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = _defaultManagerEmail };

                manager.Create(user, _defaultManagerPassword);
                manager.AddToRole(user.Id, _managerRole);
            }

            if (!context.Roles.Any(r => r.Name == _employeeRole))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = _employeeRole };

                manager.Create(role);
            }

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
