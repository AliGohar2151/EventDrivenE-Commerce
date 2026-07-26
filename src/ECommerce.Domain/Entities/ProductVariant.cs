using ECommerce.Domain.Primitives;

namespace ECommerce.Domain.Entities;

public class ProductVariant : ValueObject
{
    public ProductVariant(string variantSku, string name, decimal priceModifier)
    {
        VariantSku = variantSku;
        Name = name;
        PriceModifier = priceModifier;
    }

    public string VariantSku { get; }
    public string Name { get; }
    public decimal PriceModifier { get; }

    public override IEnumerable<object?> GetAtomicValues()
    {
        yield return VariantSku;
        yield return Name;
        yield return PriceModifier;
    }
}
