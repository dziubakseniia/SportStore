using Microsoft.AspNet.Identity.EntityFramework;

namespace SportsStore.Domain.Entities
{
    public class User : IdentityUser
    {
        public Status Status { get; set; }
    }
    public enum Status
    {
        Unlocked, Blocked
    }
}
