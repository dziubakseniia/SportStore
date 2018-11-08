using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SportsStore.Domain.Entities
{
    /// <summary>
    /// Manages Products.
    /// </summary>
    public class Product
    {
        [HiddenInput(DisplayValue = false)]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please enter a product name.")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter a price.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please specify the category")]
        public string Category { get; set; }

        [DisplayName("Date of Addition")]
        public DateTime DateOfAddition { get; set; }

        [Required(ErrorMessage = "Please enter a quantity.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a positive quantity.")]
        public int Quantity { get; set; }

        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }
}