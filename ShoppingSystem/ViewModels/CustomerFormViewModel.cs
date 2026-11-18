using System.ComponentModel.DataAnnotations;
using ShoppingSystem.ViewModels.Validation;

namespace ShoppingSystem.ViewModels
{
    public class CustomerFormViewModel
    {
        [Display(Name = "First name")]
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(30)]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(30)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50)]
        public string Address { get; set; } = string.Empty;

        [Range(NumericValidationValues.DiscountMinValue, NumericValidationValues.DiscountMaxValue,
            ErrorMessage = "Discount must be between 0 and 100.")]
        public int Discount { get; set; }
    }
}
