using System;
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
    /// <summary>
    /// Controller for Administrators for managing roles.
    /// </summary>
    [Authorize(Roles = "Administrators")]
    public class RoleAdminController : Controller
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Property for User Manager.
        /// </summary>
        private EfUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<EfUserManager>(); }
        }

        /// <summary>
        /// Property for Role Manager
        /// </summary>
        private EfRoleManager RoleManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<EfRoleManager>(); }
        }

        /// <summary>
        /// Shows Roles.
        /// </summary>
        /// <returns>View of existing Roles.</returns>
        public ActionResult Index()
        {
            ViewBag.MenuType = "Roles";
            return View(RoleManager.Roles);
        }

        /// <summary>
        /// Page for creating Roles.
        /// </summary>
        /// <returns>View for creating Roles.</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// PostBack method for creating Roles.
        /// </summary>
        /// <param name="name">string name of role to create.</param>
        /// <returns>Main Page of Roles.</returns>
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
                        _logger.Info($"Role {name} was created.");
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

        /// <summary>
        /// PostBack method for deleting Roles.
        /// </summary>
        /// <param name="id">string id of role to delete.</param>
        /// <returns>Main Page of Roles if role to delete exists.</returns>
        /// <returns>Error View if role has not been found.</returns>
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
                        _logger.Info($"Role {role.Name} was deleted.");
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

        /// <summary>
        /// Edits Role.
        /// </summary>
        /// <param name="id">string id of Role.</param>
        /// <returns>View of RoleEditModel.</returns>
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

        /// <summary>
        /// PostBack method for editing Roles.
        /// </summary>
        /// <param name="model"><c>RoleModificationModel</c> for model to add.</param>
        /// <returns>Main Page of Roles if ModelState is valid.</returns>
        /// <returns>Error View if Role has not been found.</returns>
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
                            _logger.Info($"Role {model.RoleName} was edited.");
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
                            _logger.Info($"Role {model.RoleName} was edited.");
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