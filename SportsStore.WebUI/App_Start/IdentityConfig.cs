using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SportsStore.Domain.Concrete;

namespace SportsStore.WebUI
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<EfIdentityDbContext>(EfIdentityDbContext.Create);
            app.CreatePerOwinContext<EfUserManager>(EfUserManager.Create);
            app.CreatePerOwinContext<EfRoleManager>(EfRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
        }
    }
}