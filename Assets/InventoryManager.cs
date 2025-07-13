// InventoryManager.cs
using UnityEngine;
using System.Collections.Generic; // For List<T>
using UnityEngine.UI; // For Image in ItemSlotUI (though not directly used here, good practice)
using TMPro; // For TextMeshProUGUI (though not directly used here, good practice)

public class InventoryManager : MonoBehaviour
{
    public List<Item> playerItems = new List<Item>(); // The list that holds all player's items
    public int inventorySize = 5; // Max number of inventory slots (set to 5 for your bar)

    // --- NEW: References to your manually placed slots (ASSIGN THESE IN INSPECTOR!) ---
    public InventorySlotUI[] manualUiSlots; // Drag your 5 Slot_# GameObjects here in order
    // --- END NEW ---

    // This is optional now if your bar is always present, but useful if you want to toggle it later
    public GameObject inventoryUIContainer; // Assign your BottomActionBar Panel here

    void Start()
    {
        InitializeManualSlotsUI(); // Initialize and clear your manual slots

        // Example: Add a test item automatically for quick testing
        // Ensure you have a Sprite asset in your Project window named "RedSquareIcon" or similar
        // If not, you can create one: Right-click in Project window -> Create -> 2D -> Sprites -> Square, then set its color to red.
        // Then drag that sprite into the 'itemIcon' field of the 'Test Item (Auto)' entry below in the Inspector
        Texture2D tex = new Texture2D(64, 64);
        for (int x = 0; x < 64; x++) { for (int y = 0; y < 64; y++) { tex.SetPixel(x, y, Color.red); } }
        tex.Apply();
        Sprite testIcon = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        // Add an item with a stack size > 1 to see the number change
        AddItem(new Item("Test Item (Auto)", testIcon, true, 5)); // Add a test item with quantity 5
    }

    public void AddItem(Item itemToAdd)
    {
        Debug.Log($"[InventoryManager] Attempting to add {itemToAdd.itemName} (Stack: {itemToAdd.currentStackSize}). Current inventory count: {playerItems.Count}"); // Diagnostic log

        // Basic Add (no stacking logic yet, just adds new item if space)
        if (playerItems.Count < inventorySize)
        {
            playerItems.Add(itemToAdd);
            UpdateInventoryUI(); // This will now update your manual slots
            Debug.Log($"[InventoryManager] Successfully added {itemToAdd.itemName} to inventory.");
        }
        else
        {
            Debug.Log("[InventoryManager] Inventory is full! Cannot add item.");
        }
    }

    // --- NEW: Method to initialize and update manual slots ---
    void InitializeManualSlotsUI()
    {
        if (manualUiSlots == null || manualUiSlots.Length == 0)
        {
            Debug.LogError("InventoryManager: Manual UI Slots array is empty or not assigned! Assign your Slot_1 to Slot_5 GameObjects in Inspector.");
            return;
        }
        if (manualUiSlots.Length != inventorySize)
        {
            Debug.LogWarning($"InventoryManager: Mismatch between inventorySize ({inventorySize}) and manualUiSlots count ({manualUiSlots.Length}). Adjusting inventorySize to match provided slots.");
            inventorySize = manualUiSlots.Length; // Adjust inventory size to match provided slots
        }

        // Initially clear all manual slots
        foreach (var slotUI in manualUiSlots)
        {
            if (slotUI != null) slotUI.ClearSlot();
        }
        Debug.Log("[InventoryManager] Manual UI Slots Initialized and cleared.");
    }
    // --- END NEW ---

    public void UpdateInventoryUI()
    {
        Debug.Log($"[InventoryManager] Updating UI. Player items count: {playerItems.Count}. Manual slots count: {manualUiSlots.Length}"); // Diagnostic log
        for (int i = 0; i < manualUiSlots.Length; i++) // Loop through your manual slots
        {
            if (manualUiSlots[i] == null)
            {
                Debug.LogWarning($"[InventoryManager] Slot reference at index {i} is NULL. Cannot update."); // Diagnostic log
                continue; // Skip this slot if its reference is missing
            }

            if (i < playerItems.Count)
            {
                Debug.Log($"[InventoryManager] Assigning item {playerItems[i].itemName} (Stack: {playerItems[i].currentStackSize}, Stackable: {playerItems[i].isStackable}) to slot {i} ({manualUiSlots[i].name})."); // Diagnostic log
                manualUiSlots[i].SetItem(playerItems[i]); // Display item
            }
            else
            {
                Debug.Log($"[InventoryManager] Clearing slot {i} ({manualUiSlots[i].name})."); // Diagnostic log
                manualUiSlots[i].ClearSlot(); // Clear empty slots
            }
        }
    }

    // This method is optional if your bar is always present, but useful for other UI panels
    public void ToggleInventoryUI()
    {
        if (inventoryUIContainer != null)
        {
            bool newState = !inventoryUIContainer.activeSelf;
            inventoryUIContainer.SetActive(newState);
            Time.timeScale = newState ? 0f : 1f; // Pause/Unpause game
            Debug.Log($"[InventoryManager] Inventory UI Container Toggled: {newState}");
        }
        else
        {
            Debug.LogError("InventoryManager: inventoryUIContainer (BottomActionBar) is NULL! Cannot toggle UI.");
        }
    }
}