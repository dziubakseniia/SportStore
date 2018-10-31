using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace SportsStore.Domain.Concrete
{
    public class EfRoleManager : RoleManager<EfRole>, IDisposable
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
