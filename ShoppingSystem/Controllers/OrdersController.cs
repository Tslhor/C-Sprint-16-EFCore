using Microsoft.AspNetCore.Mvc;
using ShoppingSystem.Models;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            var orders = new OrdersIndexViewModel();
            return View(orders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            CreateOrderViewModel createOrder = new CreateOrderViewModel();
            createOrder.OrderDetails.Add(new OrderItemViewModel());

            // ToDo: Add real data
            PopulateCreateOptions(createOrder);

            return View(createOrder);
        }

        [HttpPost]
        public IActionResult Create(CreateOrderViewModel createOrder)
        {
            PopulateCreateOptions(createOrder);

            if (!ModelState.IsValid)
            {
                return View(createOrder);
            }

            return View(createOrder);
        }

        private void PopulateCreateOptions(CreateOrderViewModel createOrder)
        {
            createOrder.Customers = new List<Customer>
            {
                new Customer
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Test",
                    Address = "City",
                    Discount = 5
                }
            };

            createOrder.Supermarkets = new List<Supermarket>
            {
                new Supermarket
                {
                    Id = 1,
                    Name = "Market",
                    Address = "City"
                }
            };

            createOrder.Products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Test",
                    Price = 10
                }
            };
        }
    }
}
