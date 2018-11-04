using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Identity.Concrete;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class UserAdminController : Controller
    {
        public ActionResult Users()
        {
            return View(UserManager.Users);
        }

        public EfUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<EfUserManager>(); }
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { UserName = model.UserName, Email = model.Email, Status = Status.Unlocked};
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(string id)
        {
            User user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new[] { "User not found" });
            }
        }

        public async Task<ActionResult> EditUser(string id)
        {
            User user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Users");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(string id, string email, string password)
        {
            User user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                IdentityResult validEmail = await UserManager.UserValidator.ValidateAsync(user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }

                IdentityResult validPassword = null;
                if (password != string.Empty)
                {
                    validPassword = await UserManager.PasswordValidator.ValidateAsync(password);
                    if (validPassword.Succeeded)
                    {
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPassword);
                    }
                }

                if (validPassword != null && ((validEmail.Succeeded) ||
                                              validEmail.Succeeded && password != string.Empty && validPassword.Succeeded))
                {
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["message"] = string.Format("{0} has been changed successfully", user.UserName);
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", @"User not found");
            }

            return View(user);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public async Task<ActionResult> ChangeStatus(string id)
        {
            User user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Users");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ChangeStatus(string id, Status status)
        {
            User user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Status = status;
                IdentityResult result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["message"] = string.Format("{0} has been changed successfully", user.UserName);
                    return RedirectToAction("Users");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return RedirectToAction("Users");
        }
    }
}