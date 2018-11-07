using Microsoft.AspNet.Identity.EntityFramework;

namespace SportsStore.Domain.Identity.Concrete
{
    public class EfRole : IdentityRole
    {
        public EfRole()
        { }

        public EfRole(string roleName) : base(roleName)
        { }
    }
}