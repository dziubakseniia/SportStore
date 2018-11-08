using System;
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
    /// Controller for Administrators for managing Users.
    /// </summary>
    [Authorize(Roles = "Administrators")]
    public class UserAdminController : Controller
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Property for Role Manager.
        /// </summary>
        private EfRoleManager RoleManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<EfRoleManager>(); }
        }

        /// <summary>
        /// Property for User Manager.
        /// </summary>
        private EfUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<EfUserManager>(); }
        }

        /// <summary>
        /// Shows Users.
        /// </summary>
        /// <returns>View with List of Users.</returns>
        public ActionResult Users()
        {
            ViewBag.MenuType = "Users";
            return View(UserManager.Users);
        }

        /// <summary>
        /// Creates Users.
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateUser()
        {
            return View();
        }

        /// <summary>
        /// PostBack method for creating users.
        /// </summary>
        /// <param name="model"><c>UserViewModel</c> for creating User.</param>
        /// <returns>Main Page of Users if ModelState is valid.</returns>
        /// <returns>View of model if ModelState is not valid.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { UserName = model.UserName, Email = model.Email, Status = Status.Unlocked };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                try
                {
                    if (result.Succeeded)
                    {
                        _logger.Info($"User {user.UserName} was created.");
                        return RedirectToAction("Users");
                    }

                    throw new Exception();
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.Message);
                }
            }

            return View(model);
        }

        /// <summary>
        /// PostBack method for User to delete.
        /// </summary>
        /// <param name="id">string id of User to delete.</param>
        /// <returns>Main Page of Users if User was successfully deleted.</returns>
        /// <returns>View Error if User was not found.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteUser(string id)
        {
            User user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                try
                {
                    if (result.Succeeded)
                    {
                        _logger.Info($"Role {user.UserName} was deleted.");
                        return RedirectToAction("Users");
                    }

                    throw new Exception();
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.Message);
                }
            }
            return View("Error", new[] { "User not found" });
        }

        /// <summary>
        /// Edits User.
        /// </summary>
        /// <param name="id">string id of user to edit.</param>
        /// <returns>View of User details if User exists.</returns>
        /// <returns>View of all Users if User does not exist.</returns>
        public async Task<ActionResult> EditUser(string id)
        {
            User user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }

            return RedirectToAction("Users");
        }

        /// <summary>
        /// PostBack method for editing User.
        /// </summary>
        /// <param name="id">string id of User to edit.</param>
        /// <param name="email">string email of User that should be changed.</param>
        /// <param name="password">string password of User that should be changed.</param>
        /// <returns>View of all Users if User was edited successfully.</returns>
        /// <returns>View of selected User if User was not edited.</returns>
        [HttpPost]
        public async Task<ActionResult> EditUser(string id, string email, string password)
        {
            User user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                IdentityResult validEmail = await UserManager.UserValidator.ValidateAsync(user);
                try
                {
                    if (validEmail.Succeeded)
                    {
                        IdentityResult validPassword = null;
                        if (password != string.Empty)
                        {
                            validPassword = await UserManager.PasswordValidator.ValidateAsync(password);
                            try
                            {
                                if (validPassword.Succeeded)
                                {
                                    user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
                                }

                                throw new Exception();
                            }
                            catch (Exception exception)
                            {
                                _logger.Error(exception.Message);
                            }
                        }

                        if (validPassword != null && ((validEmail.Succeeded) ||
                                                      validEmail.Succeeded && password != string.Empty &&
                                                      validPassword.Succeeded))
                        {
                            IdentityResult result = await UserManager.UpdateAsync(user);
                            try
                            {
                                if (result.Succeeded)
                                {
                                    TempData["message"] = string.Format("{0} has been changed successfully",
                                        user.UserName);
                                    _logger.Info($"User {user.UserName} was edited.");
                                    return RedirectToAction("Users");
                                }

                                throw new Exception();
                            }
                            catch (Exception exception)
                            {
                                _logger.Error(exception.Message);
                            }
                        }
                    }

                    throw new Exception();
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.Message);
                }
            }
            else
            {
                ModelState.AddModelError("", @"User not found");
            }

            return View(user);
        }

        /// <summary>
        /// Changes User's status.
        /// </summary>
        /// <param name="id">string id of User to change.</param>
        /// <returns>View of User if it exists.</returns>
        /// <returns>View of all Users if it does not exist.</returns>
        public async Task<ActionResult> ChangeStatus(string id)
        {
            User user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }

            return RedirectToAction("Users");
        }

        /// <summary>
        /// PostBack method for changing User status.
        /// </summary>
        /// <param name="id">string id of User to be changed.</param>
        /// <param name="status"><c>Status</c> of User to be changed.</param>
        /// <returns>View of all Users.</returns>
        [HttpPost]
        public async Task<ActionResult> ChangeStatus(string id, Status status)
        {
            User user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result;
                user.Status = status;
                if (user.Status == Status.Blocked)
                {
                    result = await UserManager.AddToRoleAsync(user.Id, "Blocked Users");
                    try
                    {
                        if (result.Succeeded)
                        {
                            var blockedRoles = (from role in RoleManager.Roles
                                                where role.Name != "Blocked Users"
                                                select role.Name).ToArray();

                            foreach (var role in blockedRoles)
                            {
                                result = await UserManager.RemoveFromRoleAsync(user.Id, role);
                                try
                                {
                                    if (!result.Succeeded)
                                    {
                                        throw new Exception();
                                    }
                                }
                                catch (Exception exception)
                                {
                                    _logger.Error(exception.Message);
                                }
                            }
                        }

                        throw new Exception();
                    }
                    catch (Exception exception)
                    {
                        _logger.Error(exception.Message);
                    }
                }

                if (user.Status == Status.Unlocked)
                {
                    result = await UserManager.RemoveFromRoleAsync(user.Id, "Blocked Users");
                    try
                    {
                        if (!result.Succeeded)
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Error(exception.Message);
                    }
                    result = await UserManager.AddToRoleAsync(user.Id, "Users");
                    try
                    {
                        if (!result.Succeeded)
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.Error(exception.Message);
                    }
                }
                result = await UserManager.UpdateAsync(user);
                try
                {
                    if (result.Succeeded)
                    {
                        TempData["message"] = string.Format("{0} has been changed successfully", user.UserName);
                        _logger.Info($"User's {user.UserName} status was changed.");
                        return RedirectToAction("Users");
                    }

                    throw new Exception();
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.Message);
                }
            }
            return RedirectToAction("Users");
        }
    }
}