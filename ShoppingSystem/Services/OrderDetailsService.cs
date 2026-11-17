using Microsoft.EntityFrameworkCore;
using Data;
using Domain.Entities;
using ShoppingSystem.ViewModels;

namespace ShoppingSystem.Services;

public sealed class OrderDetailsService(ShoppingDbContext context) : IOrderDetailsService
{
    public async Task<OrderDetailsFormViewModel?> BuildCreateViewModelAsync(int orderId)
    {
        var orderExists = await context.Orders.AsNoTracking()
            .AnyAsync(item => item.Id == orderId);

        if (!orderExists)
        {
            return null;
        }

        var model = new OrderDetailsFormViewModel
        {
            OrderId = orderId
        };

        await PopulateProductsAsync(model);
        return model;
    }

    public async Task PopulateProductsAsync(OrderDetailsFormViewModel model)
    {
        model.Products = await context.Products.AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<(string Key, string ErrorMessage)>> ValidateCreateAsync(OrderDetailsFormViewModel model)
    {
        var errors = new List<(string Key, string ErrorMessage)>();

        if (!await context.Orders.AnyAsync(order => order.Id == model.OrderId))
        {
            errors.Add((nameof(model.OrderId), "Selected order was not found."));
        }

        if (!await context.Products.AnyAsync(product => product.Id == model.ProductId))
        {
            errors.Add((nameof(model.ProductId), "Selected product was not found."));
        }

        if (await context.OrderDetails.AnyAsync(item =>
            item.OrderId == model.OrderId && item.ProductId == model.ProductId))
        {
            errors.Add((nameof(model.ProductId), "This product is already added to the order."));
        }

        return errors;
    }

    public async Task<OrderDetails> CreateAsync(OrderDetailsFormViewModel model)
    {
        var detail = new OrderDetails
        {
            OrderId = model.OrderId,
            ProductId = model.ProductId,
            Quantity = model.Quantity
        };

        context.OrderDetails.Add(detail);
        await context.SaveChangesAsync();

        return detail;
    }

    public async Task<OrderDetailsFormViewModel?> BuildEditViewModelAsync(int id)
    {
        var detail = await context.OrderDetails.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (detail == null)
        {
            return null;
        }

        var model = new OrderDetailsFormViewModel
        {
            Id = detail.Id,
            OrderId = detail.OrderId,
            ProductId = detail.ProductId,
            Quantity = detail.Quantity
        };

        await PopulateProductsAsync(model);
        return model;
    }

    public async Task<IReadOnlyList<(string Key, string ErrorMessage)>> ValidateEditAsync(int id, OrderDetailsFormViewModel model)
    {
        var errors = new List<(string Key, string ErrorMessage)>();

        if (!await context.Orders.AnyAsync(order => order.Id == model.OrderId))
        {
            errors.Add((nameof(model.OrderId), "Selected order was not found."));
        }

        if (!await context.Products.AnyAsync(product => product.Id == model.ProductId))
        {
            errors.Add((nameof(model.ProductId), "Selected product was not found."));
        }

        if (await context.OrderDetails.AnyAsync(item =>
            item.Id != id && item.OrderId == model.OrderId && item.ProductId == model.ProductId))
        {
            errors.Add((nameof(model.ProductId), "This product is already added to the order."));
        }

        return errors;
    }

    public async Task<bool> UpdateAsync(int id, OrderDetailsFormViewModel model)
    {
        var detail = await context.OrderDetails.FindAsync(id);
        if (detail == null)
        {
            return false;
        }

        detail.OrderId = model.OrderId;
        detail.ProductId = model.ProductId;
        detail.Quantity = model.Quantity;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<OrderDetails?> GetForDeleteAsync(int id)
    {
        return await context.OrderDetails.AsNoTracking()
            .Include(item => item.Product)
            .FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<(bool Success, int OrderId)> DeleteAsync(int id)
    {
        var detail = await context.OrderDetails.FindAsync(id);
        if (detail == null)
        {
            return (false, 0);
        }

        var orderId = detail.OrderId;
        context.OrderDetails.Remove(detail);
        await context.SaveChangesAsync();

        return (true, orderId);
    }
}
