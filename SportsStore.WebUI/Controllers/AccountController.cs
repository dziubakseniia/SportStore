using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Identity.Concrete;
using SportsStore.Domain.Migrations;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    /// <summary>
    /// Controller for accounts managing.
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Method for logging user in.
        /// </summary>
        /// <param name="returnUrl">string Url for returning after logging in.</param>
        /// <returns>Login View.</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Property for Authorization Manager.
        /// </summary>
        private IAuthenticationManager AuthManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        /// <summary>
        /// Property for User Manager.
        /// </summary>
        private EfUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<EfUserManager>(); }
        }

        /// <summary>
        /// PostBack method after logging in.
        /// </summary>
        /// <param name="details">Details for logging in.</param>
        /// <param name="returnUrl">string Url for returning after logging in.</param>
        /// <returns>Main Page if the user is in "Administrators" role.</returns>
        /// <returns>Url after logging in if it is a user.</returns>
        /// <returns>Login page if ModelState is not valid.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindAsync(details.Name, details.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", @"Invalid name or password");
                }
                else
                {
                    ClaimsIdentity identity =
                        await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                    foreach (IdentityUserRole identityUserRole in user.Roles)
                    {
                        if (identityUserRole.RoleId == "c23b7312-bd87-4dca-a1ba-ed09f8e25b09")
                        {
                            _logger.Info($"{user.UserName} logged in.");
                            return RedirectToAction("Index", "Admin");
                        }
                    }

                    _logger.Info($"{user.UserName} logged in.");
                    return Redirect(returnUrl);
                }
            }

            ViewBag.returnUrl = returnUrl;
            return View(details);
        }

        /// <summary>
        /// Loges user out.
        /// </summary>
        /// <returns>List of Products View.</returns>
        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();
            _logger.Info("User logged out.");
            return RedirectToAction("List", "Product");
        }
    }
}