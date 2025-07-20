// ShopManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// This class defines an item within the shop's inventory, linking item data with a price.
[System.Serializable]
public class ShopInventoryItem
{
    public Item itemData;
    public int price;
}

public class ShopManager : MonoBehaviour
{
    [Header("Shop Inventory")]
    public List<ShopInventoryItem> itemsForSale; // The list of items this shop sells.

    [Header("UI References")]
    public GameObject shopPanel;
    public GameObject buySection;
    public GameObject sellSection;
    public Transform buyContentArea;      // The "Content" object of the Buy section's Scroll View.
    public GameObject shopItemSlotPrefab; // The prefab for a single item slot in the shop.

    [Header("Tab Buttons")]
    public Button buyTabButton;
    public Button sellTabButton;
    
    [Header("System References")]
    public InventoryManager inventoryManager; // Assign your InventoryManager here.

    // --- Unity Methods ---
    void Start()
    {
        buyTabButton.onClick.AddListener(ShowBuySection);
        sellTabButton.onClick.AddListener(ShowSellSection);
        CloseShop();
    }

    // --- Public Methods for Opening/Closing ---
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ShowSellSection(); // Default to the "Buy" tab when opening.
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    // --- Public Method for Transactions ---
    public void BuyItem(ShopInventoryItem itemToBuy)
    {
        if (inventoryManager == null)
        {
            Debug.LogError("ShopManager: InventoryManager reference not set!");
            return;
        }

        // 1. Check if the player has enough money.
        if (inventoryManager.playerCurrency < itemToBuy.price)
        {
            inventoryManager.ShowWarning("Not Enough Money!");
            return;
        }

        // 2. Attempt to add the item to the player's inventory.
        // The AddItem function already handles checking for space.
        if (inventoryManager.AddItem(itemToBuy.itemData))
        {
            // 3. If adding was successful, complete the transaction.
            inventoryManager.playerCurrency -= itemToBuy.price;
            Debug.Log("Bought " + itemToBuy.itemData.itemName + " for " + itemToBuy.price);
        }
        // If AddItem returns false, the InventoryManager shows its own "Inventory Full!" warning.
    }

    // --- Private Methods for UI Management ---
    private void ShowBuySection()
    {
        Debug.Log("Switched to Buy tab."); // <-- ADDED THIS LINE
        buySection.SetActive(true);
        sellSection.SetActive(false);
        
        // Clear any old items first to prevent duplicates.
        foreach (Transform child in buyContentArea)
        {
            Destroy(child.gameObject);
        }

        // Create a slot for each item the shop has for sale.
        foreach (var shopItem in itemsForSale)
        {
            GameObject slotInstance = Instantiate(shopItemSlotPrefab, buyContentArea);
            ShopItemSlot slotScript = slotInstance.GetComponent<ShopItemSlot>();
            slotScript.DisplayItem(shopItem, this);
        }
    }

    private void ShowSellSection()
    {
        Debug.Log("Switched to Sell tab."); // <-- ADDED THIS LINE
        buySection.SetActive(false);
        sellSection.SetActive(true);
        // Logic to show the player's inventory will go here later.
    }
}