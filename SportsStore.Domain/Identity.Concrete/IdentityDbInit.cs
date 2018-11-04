using System.Data.Entity;

namespace SportsStore.Domain.Identity.Concrete
{
    public class IdentityDbInit : NullDatabaseInitializer<EfIdentityDbContext>
    {}
}