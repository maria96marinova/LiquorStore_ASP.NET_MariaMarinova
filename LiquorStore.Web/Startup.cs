using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LiquorStore.Web.Startup))]
namespace LiquorStore.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
