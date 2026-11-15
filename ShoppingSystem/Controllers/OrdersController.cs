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
        public IActionResult Create(
            CreateOrderViewModel createOrder,
            OrderFormCommand? command,
            int? removeIndex)
        {
            PopulateCreateOptions(createOrder);

            if (command == OrderFormCommand.AddItem)
            {
                AddItem(createOrder);
                return View(createOrder);
            }

            if (command == OrderFormCommand.RemoveItem)
            {
                RemoveItem(createOrder, removeIndex);
                return View(createOrder);
            }

            ValidateOrderForm(createOrder);

            if (!ModelState.IsValid)
            {
                return View(createOrder);
            }

            if (command == OrderFormCommand.Calculate)
            {
                CalculateTotals(createOrder);
            }

            return View(createOrder);
        }

        private void AddItem(CreateOrderViewModel model)
        {
            model.OrderDetails.Add(new OrderItemViewModel());

            ModelState.Clear();

            CalculateTotals(model);
        }

        private void RemoveItem(CreateOrderViewModel model, int? removeIndex)
        {
            if (removeIndex.HasValue &&
                removeIndex.Value >= 0 &&
                removeIndex.Value < model.OrderDetails.Count &&
                model.OrderDetails.Count > 1)
            {
                model.OrderDetails.RemoveAt(removeIndex.Value);
            }

            ModelState.Clear();

            CalculateTotals(model);
        }

        private void CalculateTotals(CreateOrderViewModel model)
        {
            model.OrderTotal = null;

            for (var index = 0; index < model.OrderDetails.Count; index++)
            {
                var item = model.OrderDetails[index];

                if (item.ProductId == 0)
                {
                    continue;
                }

                var product = model.Products.FirstOrDefault((p) => p.Id == item.ProductId);

                item.Price = null;
                item.LineTotal = null;

                if (product == null)
                {
                    ModelState.AddModelError(
                        $"OrderDetails[{index}].ProductId",
                        "Selected product was not found.");

                    continue;
                }

                item.Price = product.Price;

                item.LineTotal = item.Price * item.Quantity;
            }

            if (ModelState.IsValid)
            {
                model.OrderTotal = model.OrderDetails.Sum((o) => o.LineTotal);
            }
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

        private void ValidateOrderForm(CreateOrderViewModel model)
        {
            if (model.OrderDetails.Count == 0)
            {
                ModelState.AddModelError(
                    nameof(model.OrderDetails),
                    "Order list can not be empty.");
            }

            var productIds = new HashSet<int>();

            for (var i = 0; i < model.OrderDetails.Count; i++)
            {
                OrderItemViewModel item = model.OrderDetails[i];

                if (item.Quantity <= 0)
                {
                    ModelState.AddModelError(
                        $"OrderDetails[{i}].Quantity",
                        "Quantity must be greater than zero.");
                }

                if (item.ProductId > 0 && !productIds.Add(item.ProductId))
                {
                    ModelState.AddModelError(
                        $"OrderDetails[{i}].ProductId",
                        "This product is already added.");
                }
            }
        }
    }
}
