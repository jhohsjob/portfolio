using UnityEngine;
using UnityEngine.UI;


public class UILobbyMiddleShop : UILobbyMiddle
{
    [SerializeField]
    private VerticalLayoutGroup _content;

    protected override void Awake()
    {
        base.Awake();

        foreach (var item in ShopManager.instance.shopItemList)
        {
            var go = Instantiate(item.prefab, _content.transform);
            var uiItem = go.GetComponent<UIShopItem>();
            uiItem.Bind(item);
        }
    }
}