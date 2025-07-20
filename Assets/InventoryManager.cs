using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI; // Necessary for Image components

public class InventoryManager : MonoBehaviour
{
    // --- Friend's Addition: Player Stats ---
    [Header("Player Stats")]
    public int playerCurrency = 100; // Player starts with 100 currency
    // --- End Friend's Addition ---

    public List<Item> playerItems = new List<Item>();
    public int inventorySize = 5;
    public InventorySlotUI[] manualUiSlots;
    public GameObject inventoryUIContainer;

    [Header("Warning UI")]
    public GameObject warningUIPanel;
    public TextMeshProUGUI warningUIText;
    public float warningDisplayTime = 2.5f;
    private Coroutine warningCoroutine;

    private int currentlyEquippedSlot = -1;

    [Header("Equipped 3D Models")]
    public GameObject fishingRod3DModel; // Only the fishing rod for now

    void Start()
    {
        InitializeManualSlotsUI();

        if (warningUIPanel != null)
        {
            warningUIPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("InventoryManager: Warning UI Panel is not assigned in the Inspector. Warnings will print to console only.");
        }

        // Ensure the fishing rod model is initially hidden
        if (fishingRod3DModel != null)
        {
            fishingRod3DModel.SetActive(false);
        }

        // For initial testing, ensure you have a fishing rod in the first slot
        // REMOVE THIS BLOCK ONCE YOU HAVE PICKABLE ITEMS WORKING
        /*
        if (playerItems.Count == 0 || playerItems[0] == null || playerItems[0].itemName != "Fishing Rod")
        {
            Debug.Log("InventoryManager: Adding a Fishing Rod for testing purposes to slot 0.");
            // You'll need to assign a real Sprite for this in the Inspector of the InventoryManager
            // Or, if you have a default sprite accessible:
            // Sprite defaultRodSprite = Resources.Load<Sprite>("Sprites/FishingRodIcon"); // Example if you use Resources folder
            // Item testRod = new Item("Fishing Rod", defaultRodSprite, false, 1);
            // For now, ensure you manually add a "Fishing Rod" item to playerItems[0] in the Inspector.
        }
        */

        // Equip the first slot if the inventory is not empty
        if (playerItems.Count > 0)
        {
            EquipSlot(0);
        }
        else
        {
            currentlyEquippedSlot = -1;
            UpdateInventoryUI();
        }
    }

    void Update()
    {
        // Check for number key presses (top row OR numpad) to equip items
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) EquipSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) EquipSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) EquipSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) EquipSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) EquipSlot(4);
    }

    public void EquipSlot(int slotIndex)
    {
        Debug.Log($"[EquipSlot] Attempting to equip slot: {slotIndex}");

        if (slotIndex < 0 || slotIndex >= inventorySize)
        {
            Debug.LogError($"[EquipSlot] Failed: Index {slotIndex} is outside the inventory bounds (0-{inventorySize - 1}).");
            return;
        }

        Item itemToEquip = null;
        if (slotIndex < playerItems.Count)
        {
            itemToEquip = playerItems[slotIndex];
        }

        if (itemToEquip == null)
        {
            Debug.LogWarning($"[EquipSlot] Cannot equip empty slot {slotIndex}. No item found at this index.");
            if (currentlyEquippedSlot != -1)
            {
                manualUiSlots[currentlyEquippedSlot].Deselect();
            }
            currentlyEquippedSlot = -1;
            UpdateInventoryUI();
            // Hide the fishing rod if equipping empty slot
            if (fishingRod3DModel != null) fishingRod3DModel.SetActive(false);
            return;
        }

        if (currentlyEquippedSlot == slotIndex)
        {
            Debug.Log($"[EquipSlot] Slot {slotIndex} is already equipped. No change needed.");
            return;
        }

        Debug.Log($"[EquipSlot] Successfully selected slot {slotIndex}. Item: {itemToEquip.itemName}. Updating UI.");
        currentlyEquippedSlot = slotIndex;
        UpdateInventoryUI();

        // --- Logic to show/hide only the fishing rod ---
        if (fishingRod3DModel != null)
        {
            if (itemToEquip.itemName == "Fishing Rod") // Use the exact name from your Item class
            {
                fishingRod3DModel.SetActive(true);
                Debug.Log($"[EquipSlot] Fishing Rod 3D Model should now be active. activeSelf: {fishingRod3DModel.activeSelf}, activeInHierarchy: {fishingRod3DModel.activeInHierarchy}");
            }
            else
            {
                fishingRod3DModel.SetActive(false); // Hide rod if another item is equipped
                Debug.Log($"[EquipSlot] Fishing Rod 3D Model hidden as '{itemToEquip.itemName}' is equipped.");
            }
        }
        else
        {
            Debug.LogWarning("[EquipSlot] Fishing Rod 3D Model is NOT assigned in InventoryManager Inspector!");
        }
    }

    public Item GetEquippedItem()
    {
        if (currentlyEquippedSlot != -1 && currentlyEquippedSlot < playerItems.Count)
        {
            return playerItems[currentlyEquippedSlot];
        }
        return null;
    }

    public bool AddItem(Item itemToAdd)
    {
        // Friend's version of AddItem was simpler. Keeping your more robust version.
        if (itemToAdd.isStackable)
        {
            foreach (Item item in playerItems)
            {
                if (item.itemName == itemToAdd.itemName && item.quantity < 99)
                {
                    item.quantity += itemToAdd.quantity;
                    UpdateInventoryUI();
                    ShowWarning($"Added {itemToAdd.quantity} {itemToAdd.itemName}(s). Total: {item.quantity}");
                    return true;
                }
            }
        }

        if (playerItems.Count < inventorySize)
        {
            playerItems.Add(itemToAdd);
            UpdateInventoryUI();
            ShowWarning($"Added {itemToAdd.itemName} to inventory.");
            return true;
        }
        else
        {
            ShowWarning("Inventory Full!");
            return false;
        }
    }

    public bool RemoveItem(Item itemToRemove, int quantity = 1)
    {
        Item existingItem = playerItems.Find(item => item == itemToRemove);
        if (existingItem == null)
        {
            existingItem = playerItems.Find(item => item.itemName == itemToRemove.itemName);
        }

        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                playerItems.Remove(existingItem);
                if (currentlyEquippedSlot != -1 && manualUiSlots[currentlyEquippedSlot].GetItem() == existingItem)
                {
                    currentlyEquippedSlot = -1;
                    // Hide the fishing rod if the equipped item (which was the rod) is removed
                    if (fishingRod3DModel != null) fishingRod3DModel.SetActive(false);
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

    public void ShowWarning(string message)
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
        }

        if (warningUIPanel != null && warningUIText != null)
        {
            warningCoroutine = StartCoroutine(WarningCoroutine(message));
        }
        else
        {
            Debug.LogWarning($"[InventoryManager] Warning UI elements not assigned. Message: {message}");
        }
    }

    private IEnumerator WarningCoroutine(string message)
    {
        Debug.Log($"[InventoryManager] Displaying Warning: {message}");
        warningUIText.text = message;
        warningUIPanel.SetActive(true);
        yield return new WaitForSeconds(warningDisplayTime);
        warningUIPanel.SetActive(false);
        warningCoroutine = null;
    }

    void InitializeManualSlotsUI()
    {
        // Your version's logic for manualUiSlots.Length vs inventorySize
        if (manualUiSlots != null && manualUiSlots.Length > 0)
        {
            inventorySize = manualUiSlots.Length;
        }
        else
        {
            Debug.LogError("InventoryManager: manualUiSlots array is empty or not assigned! Inventory UI will not function.");
            inventorySize = 0;
            return;
        }

        foreach (var slotUI in manualUiSlots)
        {
            if (slotUI != null) slotUI.ClearSlot();
        }
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < manualUiSlots.Length; i++)
        {
            if (manualUiSlots[i] == null) continue; // Your safer check

            if (i < playerItems.Count && playerItems[i] != null) // Your safer check
            {
                manualUiSlots[i].SetItem(playerItems[i]);
            }
            else
            {
                manualUiSlots[i].ClearSlot();
            }

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

    public Item GetItemInSlot(int slotIndex) // Your method, kept
    {
        if (slotIndex >= 0 && slotIndex < playerItems.Count)
        {
            return playerItems[slotIndex];
        }
        return null;
    }

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
