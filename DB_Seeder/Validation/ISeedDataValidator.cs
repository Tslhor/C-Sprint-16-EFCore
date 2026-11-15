using DB_Seeder.Models;

namespace DB_Seeder.Validation;

public interface ISeedDataValidator
{
    void ValidateCustomer(CustomerSeed customer, int lineNumber);
    void ValidateSupermarket(SupermarketSeed supermarket, int lineNumber);
    void ValidateProduct(ProductSeed product, int lineNumber);
}



