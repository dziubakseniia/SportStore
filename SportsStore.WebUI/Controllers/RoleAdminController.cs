using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
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
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            ViewBag.MenuType = "Roles";
            return View(RoleManager.Roles);
        }

        private EfUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<EfUserManager>(); }
        }

        private EfRoleManager RoleManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<EfRoleManager>(); }
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
                try
                {
                    IdentityResult result = await RoleManager.CreateAsync(new EfRole(name));
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }

                    throw new Exception(result.Errors.ToString());
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.Message);
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
                try
                {
                    IdentityResult result = await RoleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }

                    throw new Exception(result.Errors.ToString());
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.Message);
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
            if (ModelState.IsValid)
            {
                IdentityResult result;
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    try
                    {
                        result = await UserManager.AddToRoleAsync(userId, model.RoleName);
                        if (result.Succeeded)
                        {
                            User user = await UserManager.FindByIdAsync(userId);
                            if (model.RoleName == "Blocked Users")
                            {
                                user.Status = Status.Blocked;
                                await UserManager.UpdateAsync(user);
                            }
                        }
                        else
                        {
                            throw new Exception(result.Errors.ToString());
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Error(exception.Message);
                    }
                }

                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    try
                    {
                        result = await UserManager.RemoveFromRoleAsync(userId, model.RoleName);
                        User user = await UserManager.FindByIdAsync(userId);
                        if (result.Succeeded)
                        {
                            if (model.RoleName == "Blocked Users")
                            {
                                user.Status = Status.Unlocked;
                                await UserManager.UpdateAsync(user);
                            }
                        }
                        else
                        {
                            throw new Exception(result.Errors.ToString());
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Error(exception.Message);
                    }
                }

                return RedirectToAction("Index");
            }

            return View("Error", new[] { "Role not found" });
        }
    }
}