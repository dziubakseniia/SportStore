using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Identity.Concrete
{
    public class EfIdentityDbContext : IdentityDbContext<User>
    {
        static EfIdentityDbContext()
        {
            Database.SetInitializer(new IdentityDbInit());
        }

        public EfIdentityDbContext() : base("IdentityDb")
        { }

        public static EfIdentityDbContext Create()
        {
            return new EfIdentityDbContext();
        }
    }
}