using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using ShoppingSystem.ViewModels.Validation;

namespace ShoppingSystem.ViewModels
{
    public class OrderItemViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Select a product.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(typeof(decimal), NumericValidationValues.ProductQuantityMinValue, NumericValidationValues.ProductQuantityMaxValue,
            ParseLimitsInInvariantCulture = true,
            ErrorMessage = "Please select quantity")]
        public decimal Quantity { get; set; }

        public decimal? Price { get; set; }
        public decimal? LineTotal { get; set; }
    }
}
