using System.ComponentModel.DataAnnotations;

namespace SportsStore.WebUI.Models
{
    /// <summary>
    /// Manages Users View.
    /// </summary>
    public class UserViewModel
    {
        [Required(ErrorMessage = "Please enter a user name.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter an email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}