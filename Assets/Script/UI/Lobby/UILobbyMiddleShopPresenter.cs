using System.Collections.Generic;
using UnityEngine.Localization.Settings;


public class UILobbyMiddleShopPresenter : UILobbyMiddlePresenter<UILobbyMiddleShop>
{
    private IReadOnlyList<ShopItemDefinition> _shopDefinitions;
    private List<UIShopItem> _createdUiItems = new();

    private bool _isInitialized = false;
    private bool _isPurchasing = false;

    public UILobbyMiddleShopPresenter(UILobbyMiddleShop view, UILobbyContext context) : base(view, context)
    {
        _shopDefinitions = context.purchaseService.shopItemList;
    }

    protected override void Bind()
    {
        _view.onInitializePanel += OnInitializePanel;
    }

    protected override void Unbind()
    {
        _view.onInitializePanel -= OnInitializePanel;
        UnbindUiItems();
    }

    public override void Initialize()
    {
    }

    private void OnInitializePanel()
    {
        if (_isInitialized == true)
        {
            return;
        }
        _isInitialized = true;

        _view.ClearItems();
        UnbindUiItems();

        foreach (var definition in _shopDefinitions)
        {
            if (definition.prefab == null)
            {
                continue;
            }

            UIShopItem uiItem = _view.CreateShopItem(definition.prefab.gameObject);
            if (uiItem == null)
            {
                continue;
            }

            uiItem.Bind(definition, id => GetPurchaseCount(id));

            uiItem.onProductClick += OnProductClickRequest;
            _createdUiItems.Add(uiItem);
        }
    }

    private void UnbindUiItems()
    {
        foreach (var uiItem in _createdUiItems)
        {
            if (uiItem != null)
            {
                uiItem.onProductClick -= OnProductClickRequest;
            }
        }
        _createdUiItems.Clear();
    }

    private int GetPurchaseCount(int productId)
    {
        var pvo = _context.purchaseService.GetProductPVOById(productId);
        return pvo?.purchaseCount ?? 0;
    }

    private async void OnProductClickRequest(ShopItemDefinition shop, ProductDefinition product, UIProduct uiTarget)
    {
        if (_isPurchasing == true)
        {
            return;
        }

        if (uiTarget == null)
        {
            return;
        }

        _isPurchasing = true;

        var result = await _context.purchaseService.Purchase(product.id);

        string productName = LocalManager.GetShopString(product.GetNameKey());
        string message = string.Empty;
        switch (result)
        {
            case PurchaseResult.Success:
                int newCount = GetPurchaseCount(product.id);
                uiTarget.UpdatePurchaseCount(newCount);

                message = LocalManager.GetShopString("success_result_message", new object[] { new { product_name = productName } });
                break;

            default:
                message = LocalManager.GetShopString("failed_result_message", new object[] { new { product_name = productName, reason = result.ToString() } });
                break;
        }

        ShowResultPopup(result, message);

        _isPurchasing = false;
    }

    private void ShowResultPopup(PurchaseResult result, string message)
    {
        _context.popupService.ShowPopup<UIProductResultPopup>(PopupName.UIProductResultPopup, new UIProductResultData
        {
            result = result,
            message = message
        });
    }
}