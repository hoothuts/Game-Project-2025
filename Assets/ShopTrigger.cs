// ShopTrigger.cs
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Find the ShopManager and tell it to open the shop
            ShopManager shopManager = FindFirstObjectByType<ShopManager>();
            if (shopManager != null)
            {
                shopManager.OpenShop();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Find the ShopManager and tell it to close the shop
            ShopManager shopManager = FindFirstObjectByType<ShopManager>();
            if (shopManager != null)
            {
                shopManager.CloseShop();
            }
        }
    }
}