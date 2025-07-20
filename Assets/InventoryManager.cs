using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // Required for TextMeshProUGUI
using UnityEngine.UI; // Required for Image (in InventorySlotUI if it's part of this file)

// Ensure this class is serializable so it can be saved and displayed in the Inspector
// If your Item class is in a separate file (Item.cs), you don't need this commented block here.
/*
[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon; // The visual representation of the item
    public bool isStackable; // Can this item stack in inventory?
    public int quantity; // The quantity of this item in a stack

    // Constructor to easily create new Item instances
    public Item(string name, Sprite itemIcon, bool stackable, int qty)
    {
        itemName = name;
        icon = itemIcon;
        isStackable = stackable;
        quantity = qty;
    }

    // You might also want a default constructor if you create Items without arguments sometimes
    public Item()
    {
        itemName = "New Item";
        icon = null;
        isStackable = false;
        quantity = 1;
    }
}
*/

// If your InventorySlotUI class is in a separate file (InventorySlotUI.cs), you don't need this commented block here.
/*
public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage; // Reference to the UI Image component for the icon
    public TextMeshProUGUI quantityText; // Reference to the TextMeshProUGUI for quantity
    public GameObject selectionHighlight; // Optional: A GameObject to show when slot is selected

    private Item currentItem; // Private field to hold the item currently in this slot

    public void SetItem(Item item)
    {
        currentItem = item; // Store the item reference

        if (item != null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = item.icon; // Assign the item's sprite
                iconImage.enabled = true; // Make sure the image is visible
            }
            else
            {
                Debug.LogWarning($"InventorySlotUI on {gameObject.name}: iconImage is not assigned!");
            }

            if (quantityText != null)
            {
                // Display quantity only if it's greater than 1 and the item is stackable
                quantityText.text = item.isStackable && item.quantity > 1 ? item.quantity.ToString() : "";
            }
            else
            {
                Debug.LogWarning($"InventorySlotUI on {gameObject.name}: quantityText is not assigned!");
            }
        }
        else // If item is null, clear the slot
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null; // Clear the item reference

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false; // Hide the image
        }
        if (quantityText != null)
        {
            quantityText.text = ""; // Clear the quantity text
        }
    }

    public void Select()
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(true);
        }
        else
        {
            // Debug.LogWarning($"InventorySlotUI on {gameObject.name}: selectionHighlight is not assigned!");
        }
    }

    public void Deselect()
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(false);
        }
    }

    public Item GetItem()
    {
        return currentItem;
    }
}
*/


public class InventoryManager : MonoBehaviour
{
    public List<Item> playerItems = new List<Item>();
    public int inventorySize = 5; // This will be updated by manualUiSlots.Length
    public InventorySlotUI[] manualUiSlots; // Array of UI elements for each slot
    public GameObject inventoryUIContainer; // The parent GameObject for the entire inventory UI

    [Header("Warning UI")]
    public GameObject warningUIPanel; // The UI panel that shows warnings
    public TextMeshProUGUI warningUIText; // The TextMeshPro text for warnings
    public float warningDisplayTime = 2.5f;
    private Coroutine warningCoroutine;

    private int currentlyEquippedSlot = -1;

    void Start()
    {
        InitializeManualSlotsUI();

        if (warningUIPanel != null)
        {
            warningUIPanel.SetActive(false); // Ensure warning panel is hidden at start
        }
        else
        {
            Debug.LogWarning("InventoryManager: Warning UI Panel is not assigned in the Inspector. Warnings will print to console only.");
        }

        // --- REMOVED: The loop that automatically fills the inventory ---
        // Your friend added this loop to fill the inventory with "Test Items" at start.
        // We're commenting it out so your inventory starts empty.
        /*
        Debug.Log("Filling inventory at start...");
        // This Texture2D is empty, which is why icons didn't show up!
        Texture2D tex = new Texture2D(64, 64);
        for (int i = 0; i < inventorySize; i++)
        {
            // You'd typically load a real sprite here, not create an empty one.
            AddItem(new Item($"Test Item {i + 1}", Sprite.Create(tex, new Rect(0,0,64,64), Vector2.one * 0.5f), true, i + 1));
        }
        */

        // Equip the first slot if the inventory is not empty
        if (playerItems.Count > 0)
        {
            EquipSlot(0);
        }
        else
        {
            // If inventory is empty, ensure no slot is selected visually
            currentlyEquippedSlot = -1;
            UpdateInventoryUI(); // Clear any lingering selections
        }
    }

    void Update()
    {
        // Input for equipping slots (1-5 keys)
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            EquipSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            EquipSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            EquipSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            EquipSlot(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            EquipSlot(4);
        }
    }

    public void EquipSlot(int slotIndex)
    {
        // Guard Clause 1: Check if index is out of bounds
        if (slotIndex < 0 || slotIndex >= inventorySize)
        {
            Debug.LogError($"[EquipSlot] Failed: Index {slotIndex} is outside the inventory bounds (0-{inventorySize - 1}).");
            return;
        }

        // Guard Clause 2: Check if the slot is empty
        if (slotIndex >= playerItems.Count || playerItems[slotIndex] == null) // Also check for null item
        {
            Debug.LogWarning($"[EquipSlot] Cannot equip empty slot {slotIndex}.");
            // If we try to equip an empty slot, deselect the current one if any
            if (currentlyEquippedSlot != -1)
            {
                manualUiSlots[currentlyEquippedSlot].Deselect();
            }
            currentlyEquippedSlot = -1; // No slot is equipped
            UpdateInventoryUI(); // Update UI to reflect no selection
            return;
        }

        // If trying to equip the same slot, do nothing
        if (currentlyEquippedSlot == slotIndex)
        {
            Debug.Log($"[EquipSlot] Slot {slotIndex} is already equipped.");
            return;
        }

        Debug.Log($"[EquipSlot] Equipping slot {slotIndex}.");
        currentlyEquippedSlot = slotIndex;
        UpdateInventoryUI(); // Update UI to show new selection
        // TODO: Add logic here to activate/deactivate the actual 3D model of the equipped item
        // For example:
        // if (playerItems[slotIndex].itemName == "Fishing Rod")
        // {
        //     // Enable your fishing rod 3D model
        // }
        // else if (playerItems[slotIndex].itemName == "Axe")
        // {
        //     // Enable your axe 3D model
        // }
        // etc.
    }

    // Returns the currently equipped item, or null if nothing is equipped
    public Item GetEquippedItem()
    {
        if (currentlyEquippedSlot != -1 && currentlyEquippedSlot < playerItems.Count)
        {
            return playerItems[currentlyEquippedSlot];
        }
        return null;
    }


    // Adds an item to the inventory
    public bool AddItem(Item itemToAdd)
    {
        // Check if there's an existing stackable item to add to
        if (itemToAdd.isStackable)
        {
            foreach (Item item in playerItems)
            {
                if (item.itemName == itemToAdd.itemName && item.quantity < 99) // Assuming max stack size 99
                {
                    item.quantity += itemToAdd.quantity;
                    UpdateInventoryUI();
                    ShowWarning($"Added {itemToAdd.quantity} {itemToAdd.itemName}(s). Total: {item.quantity}");
                    return true;
                }
            }
        }

        // If not stackable, or no existing stack, add to a new slot if space is available
        if (playerItems.Count < inventorySize)
        {
            playerItems.Add(itemToAdd);
            UpdateInventoryUI();
            ShowWarning($"Added {itemToAdd.itemName} to inventory.");
            return true;
        }
        else
        {
            ShowWarning("Inventory Full!"); // Call warning if inventory is full
            return false;
        }
    }

    // Removes an item from the inventory (by item reference or name)
    public bool RemoveItem(Item itemToRemove, int quantity = 1)
    {
        Item existingItem = playerItems.Find(item => item == itemToRemove); // Find by reference
        if (existingItem == null)
        {
            existingItem = playerItems.Find(item => item.itemName == itemToRemove.itemName); // Or by name
        }

        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                playerItems.Remove(existingItem);
                // If the removed item was equipped, deselect the slot
                if (currentlyEquippedSlot != -1 && manualUiSlots[currentlyEquippedSlot].GetItem() == existingItem) // Assuming GetItem() exists in InventorySlotUI
                {
                    currentlyEquippedSlot = -1; // Deselect
                }
            }
            UpdateInventoryUI();
            ShowWarning($"Removed {quantity} {itemToRemove.itemName}(s).");
            return true;
        }
        else
        {
            ShowWarning($"Could not find {itemToRemove.itemName} to remove.");
            return false;
        }
    }

    // Displays a warning message on the UI
    public void ShowWarning(string message)
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine); // Stop any existing warning coroutine
        }

        // Check if UI elements are assigned before trying to use them
        if (warningUIPanel != null && warningUIText != null)
        {
            warningCoroutine = StartCoroutine(WarningCoroutine(message));
        }
        else
        {
            Debug.LogWarning($"[InventoryManager] Warning UI elements not assigned. Message: {message}");
            // Fallback to console if UI not set up
        }
    }

    // Coroutine to display the warning message for a set duration
    private IEnumerator WarningCoroutine(string message)
    {
        Debug.Log($"[InventoryManager] Displaying Warning: {message}"); // Log for debugging
        warningUIText.text = message;
        warningUIPanel.SetActive(true); // Show the warning panel
        yield return new WaitForSeconds(warningDisplayTime); // Wait for the display time
        warningUIPanel.SetActive(false); // Hide the warning panel
        warningCoroutine = null; // Clear the coroutine reference
    }

    // Initializes the UI slots based on the manualUiSlots array
    void InitializeManualSlotsUI()
    {
        // Adjust inventorySize to match the number of UI slots provided
        if (manualUiSlots != null && manualUiSlots.Length > 0)
        {
            inventorySize = manualUiSlots.Length;
        }
        else
        {
            Debug.LogError("InventoryManager: manualUiSlots array is empty or not assigned! Inventory UI will not function.");
            inventorySize = 0; // Prevent errors if no slots are assigned
            return;
        }

        // Clear all slots visually at the start
        foreach (var slotUI in manualUiSlots)
        {
            if (slotUI != null) slotUI.ClearSlot();
        }
    }

    // Updates the visual state of all inventory slots
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < manualUiSlots.Length; i++)
        {
            if (manualUiSlots[i] == null) continue; // Skip if slot UI is not assigned

            if (i < playerItems.Count && playerItems[i] != null)
            {
                manualUiSlots[i].SetItem(playerItems[i]); // Set item if slot has an item
            }
            else
            {
                manualUiSlots[i].ClearSlot(); // Clear slot if no item
            }

            // Handle selection highlight
            if (i == currentlyEquippedSlot)
            {
                manualUiSlots[i].Select();
            }
            else
            {
                manualUiSlots[i].Deselect();
            }
        }
    }

    // Helper method to get the item at a specific slot index
    public Item GetItemInSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < playerItems.Count)
        {
            return playerItems[slotIndex];
        }
        return null;
    }

    // --- NEW METHOD: Toggles the visibility of the inventory UI container ---
    public void ToggleInventoryUI()
    {
        if (inventoryUIContainer != null)
        {
            bool newState = !inventoryUIContainer.activeSelf;
            inventoryUIContainer.SetActive(newState);
            Debug.Log($"Inventory UI visibility toggled to: {newState}");
        }
        else
        {
            Debug.LogError("InventoryManager: inventoryUIContainer is not assigned in the Inspector! Cannot toggle UI.");
        }
    }
}
