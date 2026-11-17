using System.ComponentModel.DataAnnotations;

namespace ShoppingSystem.ViewModels
{
    public class ProductFormViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(typeof(decimal), "0.01", "99999999.99",
            ParseLimitsInInvariantCulture = true,
            ErrorMessage = "Price must be between 0.01 and 99999999.99.")]
        public decimal Price { get; set; }
    }
}
