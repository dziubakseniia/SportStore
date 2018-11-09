using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using SportsStore.Domain.Identity.Concrete;

namespace SportsStore.WebUI.HtmlHelpers
{
    /// <summary>
    /// Helpers for Identity.
    /// </summary>
    public static class IdentityHelpers
    {
        /// <summary>
        /// Static extension method for getting User name.
        /// </summary>
        /// <param name="helper">this<c>HtmlHelper</c>.</param>
        /// <param name="id">string id of User.</param>
        /// <returns>User's name.</returns>
        public static MvcHtmlString GetUserName(this HtmlHelper helper, string id)
        {
            EfUserManager userManager = HttpContext.Current.GetOwinContext().GetUserManager<EfUserManager>();

            return new MvcHtmlString(userManager.FindByIdAsync(id).Result.UserName);
        }
    }
}