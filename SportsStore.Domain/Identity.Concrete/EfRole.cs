using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SportsStore.Domain.Concrete
{
    public class EfRole : IdentityRole
    {
        public EfRole() : base()
        {
        }

        public EfRole(string roleName) : base(roleName)
        {
        }
    }
}