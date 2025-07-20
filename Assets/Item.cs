// Item.cs
using UnityEngine;

// The [System.Serializable] attribute allows Unity to display this class
// in the Inspector and save its data.
[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public bool isStackable;
    public int quantity;

    // A constructor to easily create a new item with all its details.
    public Item(string name, Sprite itemIcon, bool stackable, int qty)
    {
        itemName = name;
        icon = itemIcon;
        isStackable = stackable;
        quantity = qty;
    }

    // A default constructor in case you need to create an empty item.
    public Item()
    {
        itemName = "New Item";
        icon = null;
        isStackable = false;
        quantity = 1;
    }
}