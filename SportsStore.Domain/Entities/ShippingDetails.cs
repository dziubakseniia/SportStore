using System.ComponentModel.DataAnnotations;

namespace SportsStore.Domain.Entities
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the first address line")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter the city name")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter a country name")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Please enter your Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public bool GiftWrap { get; set; }
    }
}
