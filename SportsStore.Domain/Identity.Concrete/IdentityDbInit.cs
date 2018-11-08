using System.Data.Entity;

namespace SportsStore.Domain.Identity.Concrete
{
    /// <summary>
    /// Initializes "IdentityDb" database.
    /// </summary>
    public class IdentityDbInit : NullDatabaseInitializer<EfIdentityDbContext>
    {}
}