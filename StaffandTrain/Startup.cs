using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StaffandTrain.Startup))]
namespace StaffandTrain
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
