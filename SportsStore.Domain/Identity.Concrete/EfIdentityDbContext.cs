using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Identity.Concrete
{
    public class EfIdentityDbContext : IdentityDbContext<User>
    {
        public EfIdentityDbContext() : base("IdentityDb")
        {
        }

        static EfIdentityDbContext()
        {
            Database.SetInitializer(new IdentityDbInit());
        }

        public static EfIdentityDbContext Create()
        {
            return new EfIdentityDbContext();
        }
    }
}
