using System.ComponentModel.DataAnnotations;

namespace SportsStore.Domain.Entities
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Your name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Your address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Your city is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Your country is required.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Your email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public bool GiftWrap { get; set; }
    }
}