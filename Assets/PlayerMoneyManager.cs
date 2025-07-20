using UnityEngine;
using TMPro; // Required for TextMeshProUGUI if you display money on UI

public class PlayerMoneyManager : MonoBehaviour
{
    // Singleton instance to allow easy access from other scripts
    public static PlayerMoneyManager Instance { get; private set; }

    [Header("Money Settings")]
    public int currentMoney = 0; // The player's current money

    [Header("UI References (Optional)")]
    public TextMeshProUGUI moneyTextUI; // Assign a TextMeshProUGUI component here to display money

    void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            // Optionally, prevent this GameObject from being destroyed when loading new scenes
            // DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        UpdateMoneyUI(); // Initialize UI display
    }

    // Adds money to the player's balance
    public void AddMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Attempted to add negative money. Use SpendMoney for spending.");
            return;
        }
        currentMoney += amount;
        UpdateMoneyUI();
        Debug.Log($"Added {amount} money. Total: {currentMoney}");
    }

    // Attempts to spend money from the player's balance
    public bool SpendMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Attempted to spend negative money. Use AddMoney for adding.");
            return false;
        }

        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            Debug.Log($"Spent {amount} money. Total: {currentMoney}");
            return true;
        }
        else
        {
            Debug.Log("Not enough money to spend " + amount + ". Current: " + currentMoney);
            return false;
        }
    }

    // Updates the UI text to show current money
    void UpdateMoneyUI()
    {
        if (moneyTextUI != null)
        {
            moneyTextUI.text = $"Money: {currentMoney}";
        }
    }
}
