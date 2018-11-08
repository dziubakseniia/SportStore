using Microsoft.AspNet.Identity.EntityFramework;

namespace SportsStore.Domain.Entities
{
    /// <summary>
    /// Enum for Users' status.
    /// </summary>
    public enum Status
    {
        Unlocked, Blocked
    }

    /// <summary>
    /// Manages Users.
    /// </summary>
    public class User : IdentityUser
    {
        public Status Status { get; set; }
    }
}