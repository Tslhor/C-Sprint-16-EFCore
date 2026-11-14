using System.ComponentModel.DataAnnotations;

namespace ShoppingSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int SupermarketId { get; set; }
        public DateTime OrderDate { get; set; }
        public Customer Customer { get; set; } = null!;
        public Supermarket Supermarket { get; set; } = null!;
        public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}