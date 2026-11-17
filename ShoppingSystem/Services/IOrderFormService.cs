using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Services;

public interface IOrderFormService
{
    void AddItem(CreateOrderViewModel model);
    void RemoveItem(CreateOrderViewModel model, int? removeIndex);
    void CalculateTotals(CreateOrderViewModel model, IList<(string Key, string ErrorMessage)> validationErrors);
    void ValidateOrderForm(CreateOrderViewModel model, IList<(string Key, string ErrorMessage)> validationErrors);
}
