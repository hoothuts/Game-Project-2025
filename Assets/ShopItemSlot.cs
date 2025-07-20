// ShopItemSlot.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Button actionButton;

    private Item item;
    private int price;
    private ShopManager shopManager;

    // This method is for displaying items to BUY
    public void DisplayItem(ShopInventoryItem shopItem, ShopManager manager)
    {
        item = shopItem.itemData;
        price = shopItem.price;
        shopManager = manager;

        // Update UI for a "Buy" slot
        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName;
        itemPriceText.text = "Price: " + price.ToString();
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
        
        actionButton.onClick.RemoveAllListeners(); // Clear old listeners before adding new one
        actionButton.onClick.AddListener(OnBuyButtonClicked);
    }

    // This method is for displaying items to SELL
    public void DisplayItem(Item playerItem, ShopManager manager)
    {
        item = playerItem;
        shopManager = manager;
        price = 10; // Placeholder sell price - you'll want a better system for this

        // Update UI for a "Sell" slot
        itemIcon.sprite = item.icon;
        itemNameText.text = item.itemName;
        itemPriceText.text = "Sell for: " + price.ToString();
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Sell";

        actionButton.onClick.RemoveAllListeners(); // Clear old listeners before adding new one
        actionButton.onClick.AddListener(OnSellButtonClicked);
    }

    private void OnBuyButtonClicked()
    {
        shopManager.BuyItem(new ShopInventoryItem { itemData = item, price = price });
    }

    private void OnSellButtonClicked()
    {
        Debug.Log("Sell button clicked for: " + item.itemName);
        // We will add the sell logic to the ShopManager next
        // shopManager.SellItem(item, price);
    }
}