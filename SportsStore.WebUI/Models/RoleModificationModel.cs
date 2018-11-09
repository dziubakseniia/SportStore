using System.ComponentModel.DataAnnotations;

namespace SportsStore.WebUI.Models
{
    /// <summary>
    /// Manages Role modification.
    /// </summary>
    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }

        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}