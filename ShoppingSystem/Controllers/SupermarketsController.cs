using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Domain.Entities;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Controllers
{
    public class SupermarketsController : Controller
    {
        private readonly ShoppingDbContext _context;

        public SupermarketsController(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var supermarkets = await _context.Supermarkets.AsNoTracking()
                .OrderBy(supermarket => supermarket.Id).ToListAsync();
            return View(supermarkets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var supermarket = await _context.Supermarkets.AsNoTracking()
                .FirstOrDefaultAsync(supermarket => supermarket.Id == id);

            if (supermarket == null)
            {
                return NotFound();
            }
            return View(supermarket);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new SupermarketFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(SupermarketFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var supermarket = new Supermarket
            {
                Name = model.Name,
                Address = model.Address
            };

            _context.Supermarkets.Add(supermarket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supermarket = await _context.Supermarkets.FindAsync(id);

            if (supermarket == null)
            {
                return NotFound();
            }
            var model = new SupermarketFormViewModel
            {
                Name = supermarket.Name,
                Address = supermarket.Address
            };

            ViewData["SupermarketId"] = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, SupermarketFormViewModel model)
        {
            var supermarket = await _context.Supermarkets.FindAsync(id);
            if (supermarket == null)
            {
                return NotFound();
            }

            ViewData["SupermarketId"] = id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            supermarket.Name = model.Name;
            supermarket.Address = model.Address;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var supermarket = await _context.Supermarkets.AsNoTracking()
                .FirstOrDefaultAsync(supermarket => supermarket.Id == id);

            if (supermarket == null)
            {
                return NotFound();
            }
            return View(supermarket);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supermarket = await _context.Supermarkets.FindAsync(id);
            if (supermarket == null)
            {
                return NotFound();
            }

            if (await _context.Orders.AnyAsync(order => order.SupermarketId == id))
            {
                ModelState.AddModelError(string.Empty, "Cannot delete a supermarket that has orders.");
                return View("Delete", supermarket);
            }

            _context.Supermarkets.Remove(supermarket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

