// PickableItem.cs
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public Item itemData; 
    public string pickerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(pickerTag))
        {
            InventoryManager inventoryManager = Object.FindFirstObjectByType<InventoryManager>();

            if (inventoryManager != null)
            {
                // --- NEW DIAGNOSTIC LINE ---
                // This will ONLY run when the player touches the item.
                Debug.Log($"[PICKUP ATTEMPT] Checking inventory state. Items: {inventoryManager.playerItems.Count}, Size: {inventoryManager.inventorySize}");
                // --- END NEW DIAGNOCTIC LINE ---
                
                if (itemData != null)
                {
                    if (inventoryManager.AddItem(itemData))
                    {
                        Debug.Log($"Successfully added {itemData.itemName} to inventory.");
                        Destroy(gameObject);
                    }
                }
                else
                {
                    Debug.LogWarning($"PickableItem on {gameObject.name} has no itemData assigned!");
                }
            }
            else
            {
                Debug.LogError("InventoryManager not found in the scene!");
            }
        }
    }
}