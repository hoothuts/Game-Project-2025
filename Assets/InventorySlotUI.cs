// InventorySlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemQuantityText;
    public Image slotBackground;
    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.white;

    void Awake()
    {
        if (itemIcon == null) Debug.LogError("InventorySlotUI: itemIcon not assigned on " + gameObject.name);
        if (itemQuantityText == null) Debug.LogError("InventorySlotUI: itemQuantityText not assigned on " + gameObject.name);
        if (slotBackground == null) Debug.LogError("InventorySlotUI: slotBackground not assigned on " + gameObject.name);
        else Deselect();
    }

    public void SetItem(Item item)
    {
        itemIcon.sprite = item.itemIcon;
        itemIcon.color = Color.white;

        if (item.isStackable && item.currentStackSize > 1)
        {
            itemQuantityText.text = item.currentStackSize.ToString();
        }
        else
        {
            itemQuantityText.text = "";
        }
    }

    public void ClearSlot()
    {
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        itemQuantityText.text = "";
    }

    public void Select()
    {
        if (slotBackground != null)
        {
            slotBackground.color = selectedColor;
        }
    }

    public void Deselect()
    {
        if (slotBackground != null)
        {
            slotBackground.color = defaultColor;
        }
    }
}