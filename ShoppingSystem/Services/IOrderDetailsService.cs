using Domain.Entities;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Services;

public interface IOrderDetailsService
{
    Task<OrderDetailsFormViewModel?> BuildCreateViewModelAsync(int orderId);
    Task PopulateProductsAsync(OrderDetailsFormViewModel model);
    Task<IReadOnlyList<(string Key, string ErrorMessage)>> ValidateCreateAsync(OrderDetailsFormViewModel model);
    Task<OrderDetails> CreateAsync(OrderDetailsFormViewModel model);
    Task<OrderDetailsFormViewModel?> BuildEditViewModelAsync(int id);
    Task<IReadOnlyList<(string Key, string ErrorMessage)>> ValidateEditAsync(int id, OrderDetailsFormViewModel model);
    Task<bool> UpdateAsync(int id, OrderDetailsFormViewModel model);
    Task<OrderDetails?> GetForDeleteAsync(int id);
    Task<(bool Success, int OrderId)> DeleteAsync(int id);
}
