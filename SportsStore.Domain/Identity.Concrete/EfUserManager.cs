using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Identity.Concrete
{
    /// <summary>
    /// Manages Users.
    /// </summary>
    public class EfUserManager : UserManager<User>
    {
        /// <summary>
        /// Constructor for EfUserManager.
        /// </summary>
        /// <param name="store">Base <c>IUserStore</c> for creating.</param>
        public EfUserManager(IUserStore<User> store) : base(store)
        { }

        /// <summary>
        /// Static method for creating EfUserManager.
        /// </summary>
        /// <param name="options">Options <c>IdentityFactoryOptions</c> for creating.</param>
        /// <param name="context">Context <c>IOwinContext</c> for creating.</param>
        /// <returns>New UserManager.</returns>
        public static EfUserManager Create(IdentityFactoryOptions<EfUserManager> options, IOwinContext context)
        {
            EfIdentityDbContext db = context.Get<EfIdentityDbContext>();
            EfUserManager userManager = new EfUserManager(new UserStore<User>(db));

            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 5,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = false,
                RequireNonLetterOrDigit = false
            };

            userManager.UserValidator = new UserValidator<User>(userManager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            return userManager;
        }
    }
}