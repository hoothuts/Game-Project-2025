// Item.cs
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName = "New Item";
    public Sprite itemIcon;
    public bool isStackable = false;
    public int currentStackSize = 1;

    public Item(string name, Sprite icon, bool stackable = false, int stackSize = 1)
    {
        itemName = name;
        itemIcon = icon;
        isStackable = stackable;
        currentStackSize = stackSize;
    }
}