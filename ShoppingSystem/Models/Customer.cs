using System.ComponentModel.DataAnnotations;

namespace ShoppingSystem.Models
{
    public class Customer
    {
        public int Id { get; set; }

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
        
        public int Discount { get; set; }
    }
}