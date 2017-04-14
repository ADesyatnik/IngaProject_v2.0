using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IngaProject.Startup))]
namespace IngaProject
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
