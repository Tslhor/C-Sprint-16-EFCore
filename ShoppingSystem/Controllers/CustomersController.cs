using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Domain.Entities;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ShoppingDbContext _context;

        public CustomersController(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchText, string? sortOrder)
        {
            searchText = searchText?.Trim();
            var query = _context.Customers.AsNoTracking();
            query = ApplyFiltersAndSorting(query, searchText, sortOrder);

            ViewData["SearchText"] = searchText;
            ViewData["SortOrder"] = sortOrder;

            var customers = await query.ToListAsync();
            return View(customers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await _context.Customers.AsNoTracking()
                .FirstOrDefaultAsync(customer => customer.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CustomerFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = new Customer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                Discount = model.Discount
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            var model = new CustomerFormViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                Discount = customer.Discount
            };

            ViewData["CustomerId"] = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, CustomerFormViewModel model)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            ViewData["CustomerId"] = id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Address = model.Address;
            customer.Discount = model.Discount;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.AsNoTracking()
                .FirstOrDefaultAsync(customer => customer.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            if (await _context.Orders.AnyAsync(order => order.CustomerId == id))
            {
                ModelState.AddModelError(string.Empty, "Cannot delete a customer who has orders.");
                return View("Delete", customer);
            }

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private IQueryable<Customer> ApplyFiltersAndSorting(
            IQueryable<Customer> query, string? searchText, string? sortOrder)
        {
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(customer =>
                    customer.FirstName.Contains(searchText) || customer.LastName.Contains(searchText));
            }

            switch (sortOrder)
            {
                case "lastName":
                    query = query.OrderBy(customer => customer.LastName);
                    break;

                case "lastName_desc":
                    query = query.OrderByDescending(customer => customer.LastName);
                    break;

                case "address":
                    query = query.OrderBy(customer => customer.Address);
                    break;

                case "address_desc":
                    query = query.OrderByDescending(customer => customer.Address);
                    break;

                default:
                    query = query.OrderBy(customer => customer.Id);
                    break;
            }

            return query;
        }
    }
}

