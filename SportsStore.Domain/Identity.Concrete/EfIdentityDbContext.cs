using Microsoft.AspNet.Identity.EntityFramework;
using SportsStore.Domain.Entities;
using System.Data.Entity;

namespace SportsStore.Domain.Concrete
{
    public class EfIdentityDbContext : IdentityDbContext<User>
    {
        public EfIdentityDbContext() : base("IdentityDb")
        {
        }

        static EfIdentityDbContext()
        {
            Database.SetInitializer<EfIdentityDbContext>(new IdentityDbInit());
        }

        public static EfIdentityDbContext Create()
        {
            return new EfIdentityDbContext();
        }
    }
}
