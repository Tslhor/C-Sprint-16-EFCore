using Microsoft.AspNetCore.Mvc;
using ShoppingSystem.Models;
using System.Diagnostics;

namespace ShoppingSystem.Controllers
{
    public class ProductsController : Controller
    {
        private List<Product> products = new List<Product>();

        public ProductsController()
        {
            Product product = new Product
            {
                Id = 1,
                Name = "Test",
                Price = 10,
            };

            products.Add(product);
        }

        public IActionResult Index()
        {
            return View(products);
        }

        public IActionResult Details(int id)
        {
            Product product = products[0];

            if (product == null)
            {
                return NotFound();
            }
            // ToDo: Add logic to find a product from DB
            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            // Add logic to create a product

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Product product = products[0];


            if (product == null)
            {
                return NotFound();
            }
            // ToDo: Add logic to find a product from DB
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            // Add logic to edit a product

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (product == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Product product = products[0];

            if (product == null)
            {
                return NotFound();
            }
            // ToDo: Add logic to find a product from DB
            return View(product);
        }

        [HttpPost]
        public IActionResult Delete(Product product)
        {
            // ToDo: Implement logic
            return RedirectToAction(nameof(Index));
        }
    }
}
