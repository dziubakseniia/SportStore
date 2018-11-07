using Microsoft.AspNet.Identity.EntityFramework;

namespace SportsStore.Domain.Entities
{
    public enum Status
    {
        Unlocked, Blocked
    }

    public class User : IdentityUser
    {
        public Status Status { get; set; }
    }
}