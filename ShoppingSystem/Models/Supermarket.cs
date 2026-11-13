using System.ComponentModel.DataAnnotations;

namespace ShoppingSystem.Models
{
    public class Supermarket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(30)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50)]
        public string Address { get; set; } = string.Empty;
    }
}