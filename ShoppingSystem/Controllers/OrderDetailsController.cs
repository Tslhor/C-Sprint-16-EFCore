using Microsoft.AspNetCore.Mvc;
using ShoppingSystem.Services;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly IOrderDetailsService _orderDetailsService;

        public OrderDetailsController(IOrderDetailsService orderDetailsService)
        {
            _orderDetailsService = orderDetailsService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int orderId)
        {
            var model = await _orderDetailsService.BuildCreateViewModelAsync(orderId);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderDetailsFormViewModel model)
        {
            await _orderDetailsService.PopulateProductsAsync(model);

            var validationErrors = await _orderDetailsService.ValidateCreateAsync(model);
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(error.Key, error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var detail = await _orderDetailsService.CreateAsync(model);

            return RedirectToAction("Index", "Orders", new { id = model.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _orderDetailsService.BuildEditViewModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, OrderDetailsFormViewModel model)
        {
            var existing = await _orderDetailsService.BuildEditViewModelAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            model.Id = id;
            await _orderDetailsService.PopulateProductsAsync(model);

            var validationErrors = await _orderDetailsService.ValidateEditAsync(id, model);
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(error.Key, error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var updated = await _orderDetailsService.UpdateAsync(id, model);
            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Orders", new { id = model.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var detail = await _orderDetailsService.GetForDeleteAsync(id);

            if (detail == null)
            {
                return NotFound();
            }

            return View(detail);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _orderDetailsService.DeleteAsync(id);
            if (!result.Success)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Orders", new { id = result.OrderId });
        }
    }
}
