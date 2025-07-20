// ShopItemSlot.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Button buyButton;

    private ShopInventoryItem currentShopItem;
    private ShopManager shopManager;

    public void DisplayItem(ShopInventoryItem shopItem, ShopManager manager)
    {
        currentShopItem = shopItem;
        shopManager = manager;

        // Update the UI elements
        itemIcon.sprite = shopItem.itemData.icon;
        itemNameText.text = shopItem.itemData.itemName;
        itemPriceText.text = "Price: " + shopItem.price.ToString();

        // Add a listener to the buy button
        buyButton.onClick.AddListener(OnBuyButtonClicked);
    }

    private void OnBuyButtonClicked()
    {
        // Tell the ShopManager to attempt the purchase
        shopManager.BuyItem(currentShopItem);
    }
}