using System.ComponentModel.DataAnnotations;
using ShoppingSystem.Models;

namespace ShoppingSystem.ViewModels
{
    public class CreateOrderViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Select a customer.")]
        public int CustomerId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a supermarket.")]
        public int SupermarketId { get; set; }

        [Display(Name = "Order Date")]
        [Required(ErrorMessage = "Order date is required.")]
        public DateTime? OrderDate { get; set; }
        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();
        public IEnumerable<Supermarket> Supermarkets { get; set; } = new List<Supermarket>();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public List<OrderItemViewModel> OrderDetails { get; set; } = new List<OrderItemViewModel>();
    }
}