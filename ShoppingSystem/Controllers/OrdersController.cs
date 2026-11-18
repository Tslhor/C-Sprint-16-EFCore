using Microsoft.AspNetCore.Mvc;
using ShoppingSystem.Services;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderFormService _orderFormService;

        public OrdersController(IOrderService orderService, IOrderFormService orderFormService)
        {
            _orderService = orderService;
            _orderFormService = orderFormService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var model = await _orderService.BuildIndexViewModelAsync(id);
            return View(model);
        }

        public IActionResult Details(int id)
        {
            return RedirectToAction(nameof(Index), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var createOrder = await _orderService.BuildCreateViewModelAsync();
            return View(createOrder);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateOrderViewModel createOrder,
            OrderFormCommand? command,
            int? removeIndex)
        {
            await _orderService.PopulateCreateOptionsAsync(createOrder);

            if (command == OrderFormCommand.AddItem)
            {
                _orderFormService.AddItem(createOrder);
                ModelState.Clear();
                return View(createOrder);
            }

            if (command == OrderFormCommand.RemoveItem)
            {
                _orderFormService.RemoveItem(createOrder, removeIndex);
                ModelState.Clear();
                return View(createOrder);
            }

            var validationErrors = new List<(string Key, string ErrorMessage)>();
            _orderFormService.ValidateOrderForm(createOrder, validationErrors);
            validationErrors.AddRange(await _orderService.ValidateCreateRelationshipsAsync(createOrder));

            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(error.Key, error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(createOrder);
            }

            if (command == OrderFormCommand.Calculate)
            {
                _orderFormService.CalculateTotals(createOrder, validationErrors);

                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError(error.Key, error.ErrorMessage);
                }

                return View(createOrder);
            }

            if (command != OrderFormCommand.Create)
            {
                return View(createOrder);
            }

            var order = await _orderService.CreateOrderAsync(createOrder, validationErrors);

            if (order == null)
            {
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError(error.Key, error.ErrorMessage);
                }

                await _orderService.PopulateCreateOptionsAsync(createOrder);
                return View(createOrder);
            }

            return RedirectToAction(nameof(Index), new { id = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _orderService.BuildEditViewModelAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewData["OrderId"] = id;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, OrderFormViewModel model)
        {
            var existing = await _orderService.BuildEditViewModelAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            ViewData["OrderId"] = id;
            await _orderService.PopulateEditOptionsAsync(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var validationErrors = await _orderService.ValidateEditRelationshipsAsync(model);
            foreach (var error in validationErrors)
            {
                ModelState.AddModelError(error.Key, error.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var updated = await _orderService.UpdateOrderAsync(id, model);
            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetOrderForDeleteAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _orderService.DeleteOrderAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

