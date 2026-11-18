using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using ShoppingSystem.ViewModels.Validation;

namespace ShoppingSystem.ViewModels
{
    public class OrderDetailsFormViewModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Order ID is required.")]
        public int OrderId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a product.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(typeof(decimal), NumericValidationValues.ProductQuantityMinValue, NumericValidationValues.ProductQuantityMaxValue,
            ParseLimitsInInvariantCulture = true,
            ErrorMessage = "Quantity must be greater than zero.")]
        public decimal Quantity { get; set; }

        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
