using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;


public class UILobbyMiddleShopPresenter : UILobbyMiddlePresenter<UILobbyMiddleShop>
{
    private IReadOnlyList<ShopItemDefinition> _shopDefinitions;
    private List<UIShopItem> _createdUiItems = new();

    private bool _isInitialized = false;
    private bool _isPurchasing = false;

    public UILobbyMiddleShopPresenter(UILobbyMiddleShop view, UILobbyContext context) : base(view, context)
    {
        _shopDefinitions = ShopManager.instance.shopItemList;
    }

    public override void Initialize()
    {
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
        var data = _context.productStorage.Get(productId);
        return data?.purchaseCount ?? 0;
    }

    private void OnProductClickRequest(ShopItemDefinition shopDef, ProductDefinition productDef, UIProduct targetProductUi)
    {
        if (_isPurchasing == true)
        {
            return;
        }

        if (targetProductUi == null)
        {
            return;
        }

        _ = ExecutePurchaseTask(shopDef, productDef, targetProductUi);
    }

    private async Task ExecutePurchaseTask(ShopItemDefinition shopDef, ProductDefinition productDef, UIProduct targetProductUi)
    {
        try
        {
            _isPurchasing = true;

            var (success, reason) = await _context.purchaseService.TryPurchase(productDef);

            if (targetProductUi == null)
            {
                return;
            }

            string productName = LocalizationSettings.StringDatabase.GetLocalizedString(LocalTable.ShopTable, productDef.GetNameKey());
            string message = string.Empty;

            if (success)
            {
                int newCount = GetPurchaseCount(productDef.id);
                targetProductUi.UpdatePurchaseCount(newCount);

                message = LocalizationSettings.StringDatabase.GetLocalizedString(
                    LocalTable.ShopTable,
                    "success_result_message",
                    new object[] { new { product_name = productName } });
            }
            else
            {
                message = LocalizationSettings.StringDatabase.GetLocalizedString(
                    LocalTable.ShopTable,
                    "failed_result_message",
                    new object[] { new { product_name = productName, reason = reason.ToMessage() } });
            }

            ShowResultPopup(success, message);
        }
        catch (Exception ex)
        {
            Debug.LogError($"구매 트랜잭션 도중 예외 발생: {ex.Message}");
        }
        finally
        {
            _isPurchasing = false;
        }
    }

    private void ShowResultPopup(bool success, string message)
    {
        _context.popupService.ShowPopup<UIProductResultPopup>(PopupName.UIProductResultPopup, new UIProductResultData
        {
            success = success,
            message = message
        });
    }
}