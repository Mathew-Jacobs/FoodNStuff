using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StoreComplete.WebMVC.Startup))]
namespace StoreComplete.WebMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
