using System.ComponentModel.DataAnnotations;

namespace SportsStore.WebUI.Models
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Please enter a user name.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter an email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}