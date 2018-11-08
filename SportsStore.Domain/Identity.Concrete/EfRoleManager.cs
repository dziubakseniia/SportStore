using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace SportsStore.Domain.Identity.Concrete
{
    /// <summary>
    /// Manages Roles.
    /// </summary>
    public class EfRoleManager : RoleManager<EfRole>
    {
        /// <summary>
        /// Constructor for creating <c>EfRoleManager</c>.
        /// </summary>
        /// <param name="store">Base <c>RoleStore</c> to create.</param>
        public EfRoleManager(RoleStore<EfRole> store) : base(store)
        { }

        /// <summary>
        /// Static method for creating <c>EfRoleManager</c>.
        /// </summary>
        /// <param name="options">Options <c>IdentityFactoryOptions</c> for creating.</param>
        /// <param name="context">Context <c>IOwinContext</c> for creating.</param>
        /// <returns>New <c>EfRoleManager.</c></returns>
        public static EfRoleManager Create(IdentityFactoryOptions<EfRoleManager> options, IOwinContext context)
        {
            return new EfRoleManager(new RoleStore<EfRole>(context.Get<EfIdentityDbContext>()));
        }
    }
}