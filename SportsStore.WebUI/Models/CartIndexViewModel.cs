using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Models
{
    /// <summary>
    /// Manages Cart and Index View.
    /// </summary>
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; }
    }
}