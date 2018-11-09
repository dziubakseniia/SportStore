using System.ComponentModel.DataAnnotations;

namespace SportsStore.WebUI.Models
{
    /// <summary>
    /// Manages Login View.
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter a user name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}