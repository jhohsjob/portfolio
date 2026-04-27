using System.Collections.Generic;


public class ShopManager : Singleton<ShopManager>
{
    /// <summary>
    /// key : product Id
    /// value : product
    /// </summary>
    //private Dictionary<int, Product> _dic = new();
    //private List<Product> _list = new();
    //public IReadOnlyList<Product> list => _list;

    private List<ShopItemDefinition> _list = new();
    public IReadOnlyList<ShopItemDefinition> shopItemList => _list;

    //public void InitProduct(Dictionary<int, ProductDefinition> productDefinitions)
    //{
    //    foreach (var definition in productDefinitions)
    //    {
    //        _dic.Add(definition.Key, new Product(definition.Value));
    //    }
    //    _list.AddRange(_dic.Values);
    //}

    public void InitShopItem(IReadOnlyList<ShopItemDefinition> shopItemDefinitions)
    {
        _list.AddRange(shopItemDefinitions);
    }
}
