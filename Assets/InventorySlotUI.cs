// InventorySlotUI.cs
using UnityEngine;
using UnityEngine.UI; // For Image component
using TMPro; // For TextMeshProUGUI component

public class InventorySlotUI : MonoBehaviour
{
    public Image itemIcon; // Drag the Image component here (the one that displays the item's picture)
    public TextMeshProUGUI itemQuantityText; // Drag the Text (TMP) component here (the one that displays the quantity)

    void Awake()
    {
        // Debug.Log($"[InventorySlotUI] Awake for {gameObject.name}. ItemIcon: {(itemIcon != null)}, QuantityText: {(itemQuantityText != null)}"); // Diagnostic log
        if (itemIcon == null) Debug.LogError("InventorySlotUI: itemIcon Image not assigned on " + gameObject.name);
        if (itemQuantityText == null) Debug.LogError("InventorySlotUI: itemQuantityText TextMeshProUGUI not assigned on " + gameObject.name);

        // We will no longer call ClearSlot() in Awake for manual slots,
        // as InventoryManager.InitializeManualSlotsUI() will handle initial clearing.
    }

    public void SetItem(Item item)
    {
        Debug.Log($"[InventorySlotUI] SetItem called for {gameObject.name}. Item: {item.itemName}, Stack: {item.currentStackSize}, Stackable: {item.isStackable}"); // Diagnostic log

        if (itemIcon != null)
        {
            itemIcon.sprite = item.itemIcon;
            itemIcon.color = Color.white; // Ensure icon is fully visible (white tint)
            // itemIcon.SetNativeSize(); // Optional: Makes icon match its original pixel size, can cause issues with layout groups
        }
        else { Debug.LogWarning($"SetItem: itemIcon is NULL for {gameObject.name}"); }

        if (itemQuantityText != null)
        {
            // Only show quantity if the item is stackable AND its current stack size is greater than 1
            if (item.isStackable && item.currentStackSize > 1)
            {
                itemQuantityText.text = item.currentStackSize.ToString();
                Debug.Log($"[InventorySlotUI] Displaying quantity: {item.currentStackSize}"); // Diagnostic log
            }
            else
            {
                itemQuantityText.text = ""; // Hide quantity for non-stackable or single items
                Debug.Log($"[InventorySlotUI] Quantity not displayed (Stackable: {item.isStackable}, Stack: {item.currentStackSize}). Setting text to empty."); // Diagnostic log
            }
        }
        else { Debug.LogWarning($"SetItem: itemQuantityText is NULL for {gameObject.name}"); }
    }

    public void ClearSlot()
    {
        Debug.Log($"[InventorySlotUI] ClearSlot called for {gameObject.name}"); // Diagnostic log
        if (itemIcon != null)
        {
            itemIcon.sprite = null; // Remove sprite
            itemIcon.color = new Color(1, 1, 1, 0); // Make transparent
        }
        else { Debug.LogWarning($"ClearSlot: itemIcon is NULL for {gameObject.name}"); }

        if (itemQuantityText != null)
        {
            itemQuantityText.text = ""; // Clear text
        }
        else { Debug.LogWarning($"ClearSlot: itemQuantityText is NULL for {gameObject.name}"); }
    }
}