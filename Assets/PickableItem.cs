using UnityEngine;

public class PickableItem : MonoBehaviour
{
    // This is the actual item data that this object represents
    public Item itemData; // You will assign the item's details here in the Inspector

    [Tooltip("The tag of the GameObject that can pick this item up (e.g., 'Player').")]
    public string pickerTag = "Player"; // Ensure your player has this tag

    // This method is called when another collider enters this object's trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider has the correct tag (e.g., "Player")
        if (other.CompareTag(pickerTag))
        {
            Debug.Log($"{other.name} entered trigger. Attempting to pick up {itemData?.itemName ?? "Unknown Item"}.");

            // Try to find the InventoryManager in the scene
            // --- MODIFIED LINE HERE ---
            InventoryManager inventoryManager = Object.FindFirstObjectByType<InventoryManager>();
            // --- END MODIFIED LINE ---

            if (inventoryManager != null)
            {
                if (itemData != null)
                {
                    inventoryManager.AddItem(itemData); // Add the item to the inventory
                    Debug.Log($"Successfully added {itemData.itemName} to inventory.");
                    Destroy(gameObject); // Destroy this pickable object from the scene
                }
                else
                {
                    Debug.LogWarning($"PickableItem on {gameObject.name} has no itemData assigned!");
                }
            }
            else
            {
                Debug.LogError("InventoryManager not found in the scene! Cannot pick up item.");
            }
        }
    }
}