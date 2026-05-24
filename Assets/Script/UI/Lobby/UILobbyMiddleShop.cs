using System;
using UnityEngine;
using UnityEngine.UI;


public class UILobbyMiddleShop : UILobbyMiddleBase
{
    [SerializeField]
    private VerticalLayoutGroup _content;

    public event Action onInitializePanel;

    protected override void OnShow()
    {
        base.OnShow();
    
        onInitializePanel?.Invoke();
    }

    public UIShopItem CreateShopItem(GameObject prefab)
    {
        GameObject go = Instantiate(prefab, _content.transform);
        return go.GetComponent<UIShopItem>();
    }

    public void ClearItems()
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }
    }
}