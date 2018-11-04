using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;

namespace SportsStore.Domain.Identity.Concrete
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
