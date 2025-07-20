// InventoryManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("Player Stats")]
    public int playerCurrency = 100; // Player starts with 100 currency

    public List<Item> playerItems = new List<Item>();
    public int inventorySize = 5;
    public InventorySlotUI[] manualUiSlots;
    public GameObject inventoryUIContainer;

    [Header("Warning UI")]
    public GameObject warningUIPanel;
    public TextMeshProUGUI warningUIText;
    public float warningDisplayTime = 2.5f;
    private Coroutine warningCoroutine;

    private int currentlyEquippedSlot = -1;

    void Start()
    {
        InitializeManualSlotsUI();
        
        if (warningUIPanel != null)
        {
            warningUIPanel.SetActive(false);
        }

        // Loop to fill the inventory at the start of the game
        // Debug.Log("Filling inventory at start...");
        // Texture2D tex = new Texture2D(64, 64);
        // for (int i = 0; i < inventorySize; i++)
        // {
        //     // The name includes 'i + 1' to make each item unique (e.g., "Test Item 1")
        //     AddItem(new Item($"Test Item {i + 1}", Sprite.Create(tex, new Rect(0,0,64,64), Vector2.one * 0.5f), true, i + 1));
        // }
        
        // EquipSlot(0); 
    }

    void Update()
    {
        // Check for number key presses (top row OR numpad) to equip items
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) EquipSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) EquipSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) EquipSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) EquipSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) EquipSlot(4);
    }

    public bool AddItem(Item itemToAdd)
    {
        if (playerItems.Count < inventorySize)
        {
            playerItems.Add(itemToAdd);
            UpdateInventoryUI();
            return true; // Report success
        }
        else
        {
            ShowWarning("Inventory Full!");
            return false; // Report failure
        }
    }
    
    public void ShowWarning(string message)
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
        }
        warningCoroutine = StartCoroutine(WarningCoroutine(message));
    }

    private IEnumerator WarningCoroutine(string message)
    {
        if (warningUIPanel != null && warningUIText != null)
        {
            warningUIText.text = message;
            warningUIPanel.SetActive(true);

            yield return new WaitForSeconds(warningDisplayTime);

            warningUIPanel.SetActive(false);
            warningCoroutine = null;
        }
        else
        {
            Debug.LogWarning(message);
        }
    }

    public void EquipSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySize) return;
        if (slotIndex >= playerItems.Count) return;

        currentlyEquippedSlot = slotIndex;
        UpdateInventoryUI();
    }

    void InitializeManualSlotsUI()
    {
        if (manualUiSlots.Length != inventorySize)
        {
            inventorySize = manualUiSlots.Length;
        }

        foreach (var slotUI in manualUiSlots)
        {
            if (slotUI != null) slotUI.ClearSlot();
        }
    }
    
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < manualUiSlots.Length; i++)
        {
            if (i < playerItems.Count)
            {
                manualUiSlots[i].SetItem(playerItems[i]);
            }
            else
            {
                manualUiSlots[i].ClearSlot();
            }

            if (i == currentlyEquippedSlot)
            {
                manualUiSlots[i].Select();
            }
            else
            {
                manualUiSlots[i].Deselect();
            }
        }
    }

    public void ToggleInventoryUI()
    {
        if (inventoryUIContainer != null)
        {
            bool newState = !inventoryUIContainer.activeSelf;
            inventoryUIContainer.SetActive(newState);
        }
        else
        {
            Debug.LogError("InventoryManager: inventoryUIContainer is not assigned in the Inspector!");
        }
    }
}