using Microsoft.AspNetCore.Mvc;
using ShoppingSystem.Models;
using System.Diagnostics;

namespace ShoppingSystem.Controllers
{
    public class CustomersController : Controller
    {
        private List<Customer> customers = new List<Customer>();

        public CustomersController()
        {
            Customer customer = new Customer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Test",
                Address = "City",
                Discount = 5
            };

            customers.Add(customer);
        }

        public IActionResult Index()
        {
            return View(customers);
        }

        public IActionResult Details(int id)
        {
            Customer customer = customers[0];

            if (customer == null)
            {
                return NotFound();
            }
            // ToDo: Add logic to find a customer from DB
            return View(customers[0]);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            // Add logic to create a customer

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Customer customer = customers[0];


            if (customer == null)
            {
                return NotFound();
            }
            // ToDo: Add logic to find a customer from DB
            return View(customers[0]);
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            // Add logic to edit a customer

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            if (customer == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Customer customer = customers[0];

            if (customer == null)
            {
                return NotFound();
            }
            // ToDo: Add logic to find a customer from DB
            return View(customers[0]);
        }

        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
            // ToDo: Implement logic
            return RedirectToAction(nameof(Index));
        }
    }
}
