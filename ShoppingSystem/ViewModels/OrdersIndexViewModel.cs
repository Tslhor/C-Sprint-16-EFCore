using ShoppingSystem.Models;

namespace ShoppingSystem.ViewModels
{
    public class OrdersIndexViewModel
    {
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        public Order? SelectedOrder { get; set; } = null;
    }
}