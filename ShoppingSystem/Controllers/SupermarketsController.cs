using Microsoft.AspNetCore.Mvc;
using ShoppingSystem.Models;
using System.Diagnostics;

namespace ShoppingSystem.Controllers
{
    public class SupermarketsController : Controller
    {
        private List<Supermarket> supermarkets = new List<Supermarket>();

        public SupermarketsController()
        {
            Supermarket supermarket = new Supermarket
            {
                Id = 1,
                Name = "Test",
                Address = "City",
            };

            supermarkets.Add(supermarket);
        }

        public IActionResult Index()
        {
            return View(supermarkets);
        }

        public IActionResult Details(int id)
        {
            Supermarket supermarket = supermarkets[0];

            if (supermarket == null)
            {
                return NotFound();
            }
            // ToDo: Add logic to find a supermarket from DB
            return View(supermarket);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Supermarket supermarket)
        {
            // Add logic to create a supermarket

            if (!ModelState.IsValid)
            {
                return View(supermarket);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Supermarket supermarket = supermarkets[0];


            if (supermarket == null)
            {
                return NotFound();
            }
            // ToDo: Add logic to find a supermarket from DB
            return View(supermarket);
        }

        [HttpPost]
        public IActionResult Edit(Supermarket supermarket)
        {
            // Add logic to edit a supermarket

            if (!ModelState.IsValid)
            {
                return View(supermarket);
            }

            if (supermarket == null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Supermarket supermarket = supermarkets[0];

            if (supermarket == null)
            {
                return NotFound();
            }
            // ToDo: Add logic to find a supermarket from DB
            return View(supermarket);
        }

        [HttpPost]
        public IActionResult Delete(Supermarket supermarket)
        {
            // ToDo: Implement logic
            return RedirectToAction(nameof(Index));
        }
    }
}
