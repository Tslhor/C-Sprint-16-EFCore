using DB_Seeder.Models;

namespace DB_Seeder.Validation;

public sealed class SeedDataValidator : ISeedDataValidator
{
    public void ValidateCustomer(CustomerSeed customer, int lineNumber)
    {
        if (string.IsNullOrWhiteSpace(customer.FirstName))
            throw new InvalidDataException($"Customer FirstName is required on line {lineNumber}.");

        if (string.IsNullOrWhiteSpace(customer.LastName))
            throw new InvalidDataException($"Customer LastName is required on line {lineNumber}.");

        if (string.IsNullOrWhiteSpace(customer.Address))
            throw new InvalidDataException($"Customer Address is required on line {lineNumber}.");

        if (customer.Discount is < 0 or > 100)
            throw new InvalidDataException($"Customer Discount must be between 0 and 100 on line {lineNumber}. Got: {customer.Discount}.");
    }

    public void ValidateSupermarket(SupermarketSeed supermarket, int lineNumber)
    {
        if (string.IsNullOrWhiteSpace(supermarket.Name))
            throw new InvalidDataException($"Supermarket Name is required on line {lineNumber}.");

        if (string.IsNullOrWhiteSpace(supermarket.Address))
            throw new InvalidDataException($"Supermarket Address is required on line {lineNumber}.");
    }

    public void ValidateProduct(ProductSeed product, int lineNumber)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new InvalidDataException($"Product Name is required on line {lineNumber}.");

        if (product.Price < 0)
            throw new InvalidDataException($"Product Price cannot be negative on line {lineNumber}. Got: {product.Price}.");
    }
}



