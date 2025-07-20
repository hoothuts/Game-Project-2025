using UnityEngine;

// Ensure this class is serializable so it can be saved and displayed in the Inspector
[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon; // The visual representation of the item
    public bool isStackable; // Can this item stack in inventory?
    public int quantity; // <-- ADD THIS LINE: The quantity of this item in a stack

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
