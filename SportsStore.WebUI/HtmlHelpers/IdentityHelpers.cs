using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using SportsStore.Domain.Identity.Concrete;

namespace SportsStore.WebUI.HtmlHelpers
{
    public static class IdentityHelpers
    {
        public static MvcHtmlString GetUserName(this HtmlHelper helper, string id)
        {
            EfUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<EfUserManager>();

            return new MvcHtmlString(userManager.FindByIdAsync(id).Result.UserName);
        }
    }
}