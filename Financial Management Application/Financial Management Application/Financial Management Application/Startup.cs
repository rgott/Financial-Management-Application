using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Financial_Management_Application.Startup))]
namespace Financial_Management_Application
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
