using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Domain.Entities;
using ShoppingSystem.Extensions;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShoppingDbContext _context;

        public ProductsController(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var model = await _context.Products.AsNoTracking()
                .OrderBy(product => product.Id)
                .ToPagedListAsync(pageNumber, 5);

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(product => product.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProductFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = new Product
            {
                Name = model.Name,
                Price = model.Price
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            var model = new ProductFormViewModel
            {
                Name = product.Name,
                Price = product.Price
            };

            ViewData["ProductId"] = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, ProductFormViewModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["ProductId"] = id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            product.Name = model.Name;
            product.Price = model.Price;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(product => product.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (await _context.OrderDetails.AnyAsync(detail => detail.ProductId == id))
            {
                ModelState.AddModelError(string.Empty, "Cannot delete a product that is used in orders.");
                return View("Delete", product);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

