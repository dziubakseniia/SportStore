using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Identity.Concrete
{
    public class EfUserManager : UserManager<User>
    {
        public EfUserManager(IUserStore<User> store) : base(store)
        { }

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