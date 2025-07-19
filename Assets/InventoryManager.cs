// InventoryManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    // ... (All your variables remain the same) ...
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
        // ... (Start method is unchanged) ...
        InitializeManualSlotsUI();
        
        if (warningUIPanel != null)
        {
            warningUIPanel.SetActive(false);
        }

        Debug.Log("Filling inventory at start...");
        Texture2D tex = new Texture2D(64, 64);
        for (int i = 0; i < inventorySize; i++)
        {
            AddItem(new Item($"Test Item {i + 1}", Sprite.Create(tex, new Rect(0,0,64,64), Vector2.one * 0.5f), true, i + 1));
        }
        
        EquipSlot(0); 
    }

    // --- MODIFIED: Added Debug Logs ---
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            Debug.Log("[Input] Key '1' pressed. Attempting to equip slot 0.");
            EquipSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            Debug.Log("[Input] Key '2' pressed. Attempting to equip slot 1.");
            EquipSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            Debug.Log("[Input] Key '3' pressed. Attempting to equip slot 2.");
            EquipSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            Debug.Log("[Input] Key '4' pressed. Attempting to equip slot 3.");
            EquipSlot(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            Debug.Log("[Input] Key '5' pressed. Attempting to equip slot 4.");
            EquipSlot(4);
        }
    }

    // --- MODIFIED: Added Debug Logs ---
    public void EquipSlot(int slotIndex)
    {
        Debug.Log($"[EquipSlot] Method called for index {slotIndex}.");

        // Guard Clause 1: Check if index is out of bounds
        if (slotIndex < 0 || slotIndex >= inventorySize)
        {
            Debug.LogError($"[EquipSlot] Failed: Index {slotIndex} is outside the inventory bounds (0-{inventorySize-1}).");
            return;
        }

        // Guard Clause 2: Check if the slot is empty
        if (slotIndex >= playerItems.Count)
        {
            Debug.LogError($"[EquipSlot] Failed: Cannot equip empty slot. Index {slotIndex} is greater than item count {playerItems.Count}.");
            return;
        }
        
        Debug.Log($"[EquipSlot] Success! Equipping slot {slotIndex}.");
        currentlyEquippedSlot = slotIndex;
        UpdateInventoryUI();
    }

    // ... (The rest of your script, AddItem, ShowWarning, etc., remains the same) ...
    public bool AddItem(Item itemToAdd)
    {
        if (playerItems.Count < inventorySize)
        {
            playerItems.Add(itemToAdd);
            UpdateInventoryUI();
            return true;
        }
        else
        {
            ShowWarning("Inventory Full!");
            return false;
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