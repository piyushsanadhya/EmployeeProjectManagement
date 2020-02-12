using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmployeeProjectManagement.Startup))]
namespace EmployeeProjectManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
