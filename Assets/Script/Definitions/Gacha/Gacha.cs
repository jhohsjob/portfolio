using UnityEngine;

public class Gacha
{
    private GachaDefinition _definition;

    public int id => _definition.id;

    public CurrencyType currencyType => _definition.currencyType;
    public int price => _definition.price;

    public GameObject scrollUI => _definition.scrollUI;
    public GameObject mainUI => _definition.mainUI;

    private GachaPVO _pvo;

    public Gacha(GachaDefinition definition)
    {
        _definition = definition;
    }

    public void ApplyPVO(GachaPVO pvo)
    {
        _pvo = new GachaPVO(pvo);
    }
}
