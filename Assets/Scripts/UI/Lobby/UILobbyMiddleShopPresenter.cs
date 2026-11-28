using System.Collections.Generic;


public class UILobbyMiddleShopPresenter : UILobbyMiddlePresenter<UILobbyMiddleShop>
{
    private IReadOnlyList<ShopItemDefinition> _shopDefinitions;
    private List<UIShopItem> _createdUiItems = new();

    private bool _isInitialized = false;
    private bool _isPurchasing = false;

    public UILobbyMiddleShopPresenter(UILobbyMiddleShop view, UILobbyPresenter.Deps deps) : base(view, deps)
    {
        _shopDefinitions = deps.purchaseService.shopItemList;
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

            uiItem.Bind(_deps.purchaseService, definition);

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

    private async void OnProductClickRequest(ProductDefinition product)
    {
        if (_isPurchasing == true)
        {
            return;
        }
        _isPurchasing = true;

        var result = await _deps.purchaseService.Purchase(product.Id);

        string productName = LocalizationHelper.GetShopString(product.GetNameKey());
        string message = string.Empty;
        switch (result)
        {
            case PurchaseResult.Success:
                message = LocalizationHelper.GetShopString("success_result_message", new object[] { new { product_name = productName } });
                break;

            default:
                message = LocalizationHelper.GetShopString("failed_result_message", new object[] { new { product_name = productName, reason = result.ToString() } });
                break;
        }

        ShowResultPopup(result, message);

        _isPurchasing = false;
    }

    private void ShowResultPopup(PurchaseResult result, string message)
    {
        _deps.popupService.ShowPopup<UIProductResultPopup>(PopupName.UIProductResultPopup, new UIProductResultData
        {
            result = result,
            message = message
        });
    }
}