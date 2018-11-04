using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace SportsStore.Domain.Identity.Concrete
{
    public class EfRoleManager : RoleManager<EfRole>
    {
        public EfRoleManager(RoleStore<EfRole> store) : base(store)
        {
        }

        public static EfRoleManager Create(IdentityFactoryOptions<EfRoleManager> options, IOwinContext context)
        {
            return new EfRoleManager(new RoleStore<EfRole>(context.Get<EfIdentityDbContext>()));
        }
    }
}