// Item.cs
using UnityEngine;

// We make it [System.Serializable] so Unity can display it in the Inspector
// and store it in lists easily.
[System.Serializable]
public class Item
{
    public string itemName = "New Item";
    public Sprite itemIcon; // Assign an image for the item in Unity
    public bool isStackable = false; // Can this item stack (e.g., multiple fish, one slot)
    public int currentStackSize = 1; // Current quantity if stackable

    // Constructor to easily create new items
    public Item(string name, Sprite icon, bool stackable = false, int stackSize = 1)
    {
        itemName = name;
        itemIcon = icon;
        isStackable = stackable;
        currentStackSize = stackSize;
    }
}