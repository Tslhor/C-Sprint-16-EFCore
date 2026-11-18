using ShoppingSystem.ViewModels;
using ShoppingSystem.ViewModels.Validation;

namespace ShoppingSystem.Services;

public sealed class OrderFormService : IOrderFormService
{
    public void AddItem(CreateOrderViewModel model)
    {
        model.OrderDetails.Add(new OrderItemViewModel());
        CalculateTotals(model, []);
    }

    public void RemoveItem(CreateOrderViewModel model, int? removeIndex)
    {
        if (removeIndex.HasValue &&
            removeIndex.Value >= 0 &&
            removeIndex.Value < model.OrderDetails.Count &&
            model.OrderDetails.Count > 1)
        {
            model.OrderDetails.RemoveAt(removeIndex.Value);
        }

        CalculateTotals(model, []);
    }

    public void CalculateTotals(CreateOrderViewModel model, IList<(string Key, string ErrorMessage)> validationErrors)
    {
        model.OrderTotal = null;

        for (var index = 0; index < model.OrderDetails.Count; index++)
        {
            var item = model.OrderDetails[index];

            if (item.ProductId == 0)
            {
                item.Price = null;
                item.LineTotal = null;
                continue;
            }

            var product = model.Products.FirstOrDefault(product => product.Id == item.ProductId);

            item.Price = null;
            item.LineTotal = null;

            if (product == null)
            {
                validationErrors.Add(($"OrderDetails[{index}].ProductId", "Selected product was not found."));
                continue;
            }

            item.Price = product.Price;
            item.LineTotal = item.Price * item.Quantity;
        }

        if (validationErrors.Count == 0)
        {
            model.OrderTotal = model.OrderDetails.Sum(item => item.LineTotal);
        }
    }

    public void ValidateOrderForm(CreateOrderViewModel model, IList<(string Key, string ErrorMessage)> validationErrors)
    {
        if (model.OrderDetails.Count == 0)
        {
            validationErrors.Add((nameof(model.OrderDetails), "Order list can not be empty."));
        }

        var productIds = new HashSet<int>();

        for (var i = 0; i < model.OrderDetails.Count; i++)
        {
            var item = model.OrderDetails[i];

            if (item.Quantity < NumericValidationValues.ProductQuantityMinDecimalValue)
            {
                validationErrors.Add(($"OrderDetails[{i}].Quantity", "Please select quantity"));
            }

            if (item.ProductId > 0 && !productIds.Add(item.ProductId))
            {
                validationErrors.Add(($"OrderDetails[{i}].ProductId", "This product is already added."));
            }
        }
    }
}
