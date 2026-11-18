namespace ShoppingSystem.ViewModels.Validation;

public static class NumericValidationValues
{
    public const string ProductQuantityMinValue = "0.001";
    public const string ProductQuantityMaxValue = "9999999.999";
    public const decimal ProductQuantityMinDecimalValue = 0.001m;

    public const string PriceMinValue = "0.01";
    public const string PriceMaxValue = "99999999.99";

    public const int DiscountMinValue = 0;
    public const int DiscountMaxValue = 100;
}
