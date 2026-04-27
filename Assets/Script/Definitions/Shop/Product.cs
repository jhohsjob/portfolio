public class Product
{
    private ProductDefinition _definition;
    public int id => _definition.id;
    public string name => _definition.productName;
    public CurrencyType currencyType => _definition.currencyType;
    public int price => _definition.price;

    public Product(ProductDefinition definition)
    {
        _definition = definition;
    }
}
