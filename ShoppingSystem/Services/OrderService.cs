using Microsoft.EntityFrameworkCore;
using Data;
using Domain.Entities;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Services;

public sealed class OrderService(ShoppingDbContext context) : IOrderService
{
    public async Task<OrdersIndexViewModel> BuildIndexViewModelAsync(int? selectedOrderId)
    {
        var orders = await context.Orders.AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.Supermarket)
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync();

        Order? selectedOrder = null;

        if (selectedOrderId.HasValue)
        {
            selectedOrder = await context.Orders.AsNoTracking()
                .Include(order => order.Customer)
                .Include(order => order.Supermarket)
                .Include(order => order.OrderDetails)
                    .ThenInclude(detail => detail.Product)
                .FirstOrDefaultAsync(order => order.Id == selectedOrderId.Value);
        }

        return new OrdersIndexViewModel
        {
            Orders = orders,
            SelectedOrder = selectedOrder
        };
    }

    public async Task<CreateOrderViewModel> BuildCreateViewModelAsync()
    {
        var model = new CreateOrderViewModel
        {
            OrderDate = DateTime.Today,
            OrderDetails = [new OrderItemViewModel()]
        };

        await PopulateCreateOptionsAsync(model);
        return model;
    }

    public async Task PopulateCreateOptionsAsync(CreateOrderViewModel model)
    {
        model.Customers = await context.Customers.AsNoTracking()
            .OrderBy(customer => customer.LastName)
            .ThenBy(customer => customer.FirstName)
            .ToListAsync();

        model.Supermarkets = await context.Supermarkets.AsNoTracking()
            .OrderBy(supermarket => supermarket.Name)
            .ToListAsync();

        model.Products = await context.Products.AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<(string Key, string ErrorMessage)>> ValidateCreateRelationshipsAsync(CreateOrderViewModel model)
    {
        var errors = new List<(string Key, string ErrorMessage)>();

        if (!await context.Customers.AnyAsync(customer => customer.Id == model.CustomerId))
        {
            errors.Add((nameof(model.CustomerId), "Selected customer was not found."));
        }

        if (!await context.Supermarkets.AnyAsync(supermarket => supermarket.Id == model.SupermarketId))
        {
            errors.Add((nameof(model.SupermarketId), "Selected supermarket was not found."));
        }

        var selectedProductIds = model.OrderDetails
            .Where(item => item.ProductId > 0)
            .Select(item => item.ProductId)
            .Distinct()
            .ToList();

        if (selectedProductIds.Count == 0)
        {
            return errors;
        }

        var existingProductIds = await context.Products.AsNoTracking()
            .Where(product => selectedProductIds.Contains(product.Id))
            .Select(product => product.Id)
            .ToListAsync();

        for (var index = 0; index < model.OrderDetails.Count; index++)
        {
            var item = model.OrderDetails[index];
            if (item.ProductId > 0 && !existingProductIds.Contains(item.ProductId))
            {
                errors.Add(($"OrderDetails[{index}].ProductId", "Selected product was not found."));
            }
        }

        return errors;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderViewModel model)
    {
        var order = new Order
        {
            CustomerId = model.CustomerId,
            SupermarketId = model.SupermarketId,
            OrderDate = model.OrderDate!.Value
        };

        foreach (var item in model.OrderDetails)
        {
            order.OrderDetails.Add(new OrderDetails
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        return order;
    }

    public async Task<OrderFormViewModel?> BuildEditViewModelAsync(int id)
    {
        var order = await context.Orders.AsNoTracking()
            .FirstOrDefaultAsync(order => order.Id == id);

        if (order == null)
        {
            return null;
        }

        var model = new OrderFormViewModel
        {
            CustomerId = order.CustomerId,
            SupermarketId = order.SupermarketId,
            OrderDate = order.OrderDate
        };

        await PopulateEditOptionsAsync(model);
        return model;
    }

    public async Task PopulateEditOptionsAsync(OrderFormViewModel model)
    {
        model.Customers = await context.Customers.AsNoTracking()
            .OrderBy(customer => customer.LastName)
            .ThenBy(customer => customer.FirstName)
            .ToListAsync();

        model.Supermarkets = await context.Supermarkets.AsNoTracking()
            .OrderBy(supermarket => supermarket.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<(string Key, string ErrorMessage)>> ValidateEditRelationshipsAsync(OrderFormViewModel model)
    {
        var errors = new List<(string Key, string ErrorMessage)>();

        if (!await context.Customers.AnyAsync(customer => customer.Id == model.CustomerId))
        {
            errors.Add((nameof(model.CustomerId), "Selected customer was not found."));
        }

        if (!await context.Supermarkets.AnyAsync(supermarket => supermarket.Id == model.SupermarketId))
        {
            errors.Add((nameof(model.SupermarketId), "Selected supermarket was not found."));
        }

        return errors;
    }

    public async Task<bool> UpdateOrderAsync(int id, OrderFormViewModel model)
    {
        var order = await context.Orders.FindAsync(id);
        if (order == null)
        {
            return false;
        }

        order.CustomerId = model.CustomerId;
        order.SupermarketId = model.SupermarketId;
        order.OrderDate = model.OrderDate!.Value;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Order?> GetOrderForDeleteAsync(int id)
    {
        return await context.Orders.AsNoTracking()
            .Include(item => item.Customer)
            .Include(item => item.Supermarket)
            .FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await context.Orders.FindAsync(id);
        if (order == null)
        {
            return false;
        }

        context.Orders.Remove(order);
        await context.SaveChangesAsync();
        return true;
    }
}
