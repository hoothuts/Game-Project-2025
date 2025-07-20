using UnityEngine;
using UnityEngine.UI; // Required for Image
using TMPro; // Required for TextMeshProUGUI

public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public GameObject selectionHighlight;

    private Item currentItem;

    public void SetItem(Item item)
    {
        currentItem = item; // Store the item reference

        Debug.Log($"[InventorySlotUI - {gameObject.name}] SetItem called. Item: {item?.itemName ?? "NULL"}");

        if (item != null)
        {
            if (iconImage != null)
            {
                Debug.Log($"[InventorySlotUI - {gameObject.name}] Setting icon. Item Icon is: {item.icon?.name ?? "NULL SPRITE"}");
                iconImage.sprite = item.icon; // Assign the item's sprite
                iconImage.enabled = true; // Make sure the image is visible
                Debug.Log($"[InventorySlotUI - {gameObject.name}] Icon Image enabled: {iconImage.enabled}");
            }
            else
            {
                Debug.LogWarning($"InventorySlotUI on {gameObject.name}: iconImage is not assigned!");
            }

            if (quantityText != null)
            {
                quantityText.text = item.isStackable && item.quantity > 1 ? item.quantity.ToString() : "";
                Debug.Log($"[InventorySlotUI - {gameObject.name}] Quantity Text set to: '{quantityText.text}' (Item Quantity: {item.quantity})");
            }
            else
            {
                Debug.LogWarning($"InventorySlotUI on {gameObject.name}: quantityText is not assigned!");
            }
        }
        else // If item is null, clear the slot
        {
            Debug.Log($"[InventorySlotUI - {gameObject.name}] Item is NULL, clearing slot.");
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false; // Hide the image
            Debug.Log($"[InventorySlotUI - {gameObject.name}] Slot cleared. Icon Image enabled: {iconImage.enabled}");
        }
        if (quantityText != null)
        {
            quantityText.text = "";
        }
    }

    // ... (Select, Deselect, GetItem methods remain unchanged) ...
    public void Select()
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(true);
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
