using Microsoft.AspNet.Identity.EntityFramework;

namespace SportsStore.Domain.Identity.Concrete
{
    /// <summary>
    /// Manages Roles.
    /// </summary>
    public class EfRole : IdentityRole
    {
        /// <summary>
        /// Constructor for <c>EfRole</c>.
        /// </summary>
        public EfRole()
        { }

        /// <summary>
        /// Constructor for <c>EfRole</c>.
        /// </summary>
        /// <param name="roleName">Name of the role to create.</param>
        public EfRole(string roleName) : base(roleName)
        { }
    }
}