// ShopManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using StarterAssets;

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
    public List<ShopInventoryItem> itemsForSale;

    [Header("UI References")]
    public GameObject shopPanel;
    public GameObject buySection;
    public GameObject sellSection;
    public Transform buyContentArea;
    public Transform sellContentArea; // The "Content" object of your Sell section's Scroll View
    public GameObject shopItemSlotPrefab;

    [Header("Tab Buttons")]
    public Button buyTabButton;
    public Button sellTabButton;
    private List<Button> tabButtons;
    private int selectedTabIndex = 0;
    
    [Header("System References")]
    public InventoryManager inventoryManager;

    // --- Unity Methods ---
    void Start()
    {
        // Setup button listeners for mouse clicks
        if (buyTabButton != null) buyTabButton.onClick.AddListener(ShowBuySection);
        if (sellTabButton != null) sellTabButton.onClick.AddListener(ShowSellSection);

        // Setup list for keyboard navigation
        tabButtons = new List<Button> { buyTabButton, sellTabButton };

        CloseShop();
    }

    void Update()
    {
        // Only listen for keyboard input if the shop panel is active
        if (!shopPanel.activeSelf) return;

        // Navigate with Arrow Keys
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedTabIndex = (selectedTabIndex == 0) ? 1 : 0;
            SelectTab(selectedTabIndex);
        }

        // "Click" the selected button with Enter
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (tabButtons != null && tabButtons.Count > selectedTabIndex)
            {
                tabButtons[selectedTabIndex].onClick.Invoke();
            }
        }
        
        // Close with Escape key
        if (Input.GetKeyDown(KeyCode.Backspace))
    {
        CloseShop();
    }
    }

    // --- Public Methods ---
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        
        PlayerInputManager inputManager = FindFirstObjectByType<PlayerInputManager>();
        if (inputManager != null)
        {
            inputManager.canPlayerMove = false;
        }
        
        selectedTabIndex = 0;
        SelectTab(selectedTabIndex);
        ShowBuySection();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        
        PlayerInputManager inputManager = FindFirstObjectByType<PlayerInputManager>();
        if (inputManager != null)
        {
            inputManager.canPlayerMove = true;
        }
        
        if(EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    
    public void BuyItem(ShopInventoryItem itemToBuy)
    {
        if (inventoryManager == null) return;

        if (inventoryManager.playerCurrency < itemToBuy.price)
        {
            inventoryManager.ShowWarning("Not Enough Money!");
            return;
        }

        if (inventoryManager.AddItem(itemToBuy.itemData))
        {
            inventoryManager.playerCurrency -= itemToBuy.price;
        }
    }

    // --- Private Methods ---
    private void ShowBuySection()
    {
        Debug.Log("Switched to Buy tab.");
        buySection.SetActive(true);
        sellSection.SetActive(false);
        
        foreach (Transform child in buyContentArea)
        {
            Destroy(child.gameObject);
        }
        foreach (var shopItem in itemsForSale)
        {
            GameObject slotInstance = Instantiate(shopItemSlotPrefab, buyContentArea);
            ShopItemSlot slotScript = slotInstance.GetComponent<ShopItemSlot>();
            slotScript.DisplayItem(shopItem, this);
        }
    }

    private void ShowSellSection()
    {
        Debug.Log("Switched to Sell tab.");
        buySection.SetActive(false);
        sellSection.SetActive(true);

        foreach (Transform child in sellContentArea)
        {
            Destroy(child.gameObject);
        }

        if (inventoryManager == null) return;

        foreach (var playerItem in inventoryManager.playerItems)
        {
            GameObject slotInstance = Instantiate(shopItemSlotPrefab, sellContentArea);
            ShopItemSlot slotScript = slotInstance.GetComponent<ShopItemSlot>();
            slotScript.DisplayItem(playerItem, this);
        }
    }

    private void SelectTab(int tabIndex)
    {
        if (EventSystem.current != null && tabButtons != null && tabIndex >= 0 && tabIndex < tabButtons.Count)
        {
            EventSystem.current.SetSelectedGameObject(tabButtons[tabIndex].gameObject);
            Debug.Log($"Selected tab: {tabButtons[tabIndex].name}");
        }
    }
}