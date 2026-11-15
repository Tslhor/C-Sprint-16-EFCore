using System.ComponentModel.DataAnnotations;
using ShoppingSystem.Models;

namespace ShoppingSystem.ViewModels
{
    public class OrderItemViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Select a product.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public decimal Quantity { get; set; }

        public decimal? Price { get; set; }
        public decimal? LineTotal { get; set; }
    }
}