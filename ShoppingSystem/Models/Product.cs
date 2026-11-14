using System.ComponentModel.DataAnnotations;

namespace ShoppingSystem.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }
    }
}