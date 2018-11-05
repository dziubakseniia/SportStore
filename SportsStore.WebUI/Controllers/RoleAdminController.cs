using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    public class RoleAdminController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.MenuType = "Roles";
            return View(RoleManager.Roles);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await RoleManager.CreateAsync(new EfRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            EfRole role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            return View("Error", new[] { "Role not found" });
        }

        public async Task<ActionResult> Edit(string id)
        {
            EfRole role = await RoleManager.FindByIdAsync(id);
            string[] membersIds = role.Users.Select(u => u.UserId).ToArray();
            IEnumerable<User> members = UserManager.Users.Where(x => membersIds.Any(m => m == x.Id));
            IEnumerable<User> nonMembers = UserManager.Users.Except(members);
            return View(new RoleEditModel
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RoleModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result = await UserManager.AddToRoleAsync(userId, model.RoleName);
                    User user = await UserManager.FindByIdAsync(userId);
                    if (model.RoleName == "Blocked Users")
                    {
                        user.Status = Status.Blocked;
                        await UserManager.UpdateAsync(user);
                    }
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }

                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    result = await UserManager.RemoveFromRoleAsync(userId, model.RoleName);
                    User user = await UserManager.FindByIdAsync(userId);
                    if (model.RoleName == "Blocked Users")
                    {
                        user.Status = Status.Unlocked;
                        await UserManager.UpdateAsync(user);
                    }
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }

                return RedirectToAction("Index");
            }

            return View("Error", new[] { "Role not found" });
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private EfUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<EfUserManager>(); }
        }

        private EfRoleManager RoleManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<EfRoleManager>(); }
        }

        public User CurrentUserManager
        {
            get { return System.Web.HttpContext.Current.GetOwinContext().GetUserManager<EfUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId()); }
        }
    }
}