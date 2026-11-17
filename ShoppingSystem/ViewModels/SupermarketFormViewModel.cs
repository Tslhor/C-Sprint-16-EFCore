using System.ComponentModel.DataAnnotations;

namespace ShoppingSystem.ViewModels
{
    public class SupermarketFormViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(30)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50)]
        public string Address { get; set; } = string.Empty;
    }
}
