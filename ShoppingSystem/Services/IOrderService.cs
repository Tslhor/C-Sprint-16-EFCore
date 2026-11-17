using Domain.Entities;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Services;

public interface IOrderService
{
    Task<OrdersIndexViewModel> BuildIndexViewModelAsync(int? selectedOrderId);
    Task<CreateOrderViewModel> BuildCreateViewModelAsync();
    Task PopulateCreateOptionsAsync(CreateOrderViewModel model);
    Task<IReadOnlyList<(string Key, string ErrorMessage)>> ValidateCreateRelationshipsAsync(CreateOrderViewModel model);
    Task<Order> CreateOrderAsync(CreateOrderViewModel model);
    Task<OrderFormViewModel?> BuildEditViewModelAsync(int id);
    Task PopulateEditOptionsAsync(OrderFormViewModel model);
    Task<IReadOnlyList<(string Key, string ErrorMessage)>> ValidateEditRelationshipsAsync(OrderFormViewModel model);
    Task<bool> UpdateOrderAsync(int id, OrderFormViewModel model);
    Task<Order?> GetOrderForDeleteAsync(int id);
    Task<bool> DeleteOrderAsync(int id);
}
