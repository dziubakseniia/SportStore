using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Identity.Concrete
{
    /// <summary>
    /// Manages "IdentityDb" database.
    /// </summary>
    public class EfIdentityDbContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Static constructor for managing "IdentityDb" context.
        /// </summary>
        static EfIdentityDbContext()
        {
            Database.SetInitializer(new IdentityDbInit());
        }

        /// <summary>
        /// Constructor for managing "IdentityDb" context.
        /// </summary>
        public EfIdentityDbContext() : base("IdentityDb")
        { }

        /// <summary>
        /// Static method for creating "IdentityDb" context. 
        /// </summary>
        /// <returns>New instance of "IdentityDb" context.</returns>
        public static EfIdentityDbContext Create()
        {
            return new EfIdentityDbContext();
        }
    }
}