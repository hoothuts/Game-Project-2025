using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUIController : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public InventoryManager inventoryManager; // Assign this in the Inspector

    // --- References for Key Remapping UI Elements ---
    public Button forwardKeyButtonRef;
    public TextMeshProUGUI forwardKeyText;
    public Button backwardKeyButtonRef;
    public TextMeshProUGUI backwardKeyText;
    public Button leftKeyButtonRef;
    public TextMeshProUGUI leftKeyText;
    public Button rightKeyButtonRef;
    public TextMeshProUGUI rightKeyText;
    public Button sprintKeyButtonRef;
    public TextMeshProUGUI sprintKeyText;
    public Button jumpKeyButtonRef;
    public TextMeshProUGUI jumpKeyText;


    private bool isGamePaused = false;
    private bool isWaitingForKeyPress = false;
    private string keyToRemap;

    void Start()
    {
        Debug.Log("GameUIController script has initialized and is running.");

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        // --- CODE-BASED BUTTON LINKING FOR REMAP KEYS ---
        // NOTE: This will not work with Starter Assets. See notes below the script.
        if (forwardKeyButtonRef != null) forwardKeyButtonRef.onClick.AddListener(StartRemapForwardKey);
        if (backwardKeyButtonRef != null) backwardKeyButtonRef.onClick.AddListener(StartRemapBackwardKey);
        if (leftKeyButtonRef != null) leftKeyButtonRef.onClick.AddListener(StartRemapLeftKey);
        if (rightKeyButtonRef != null) rightKeyButtonRef.onClick.AddListener(StartRemapRightKey);
        if (sprintKeyButtonRef != null) sprintKeyButtonRef.onClick.AddListener(StartRemapSprintKey);
        if (jumpKeyButtonRef != null) jumpKeyButtonRef.onClick.AddListener(StartRemapJumpKey);

        // This line ensures the cursor is locked at the start of the game
        ResumeGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isWaitingForKeyPress)
            {
                CancelRemap();
            }
            else if (optionsPanel != null && optionsPanel.activeSelf)
            {
                CloseOptionsAndReturnToPause();
            }
            else if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryManager != null)
            {
                inventoryManager.ToggleInventoryUI();
            }
            else
            {
                Debug.LogError("InventoryManager reference is NULL in GameUIController!");
            }
        }
    }

    public void PauseGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        Time.timeScale = 0f;
        isGamePaused = true;
        Debug.Log("Game Paused");

        // --- ADDED FOR STARTER ASSETS ---
        // Unlock the cursor and make it visible to interact with the UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // --- END ADDED CODE ---
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        isGamePaused = false;
        Debug.Log("Game Resumed");

        // --- ADDED FOR STARTER ASSETS ---
        // Lock the cursor and hide it for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // --- END ADDED CODE ---
    }

    public void OpenOptionsFromPause()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void CloseOptionsAndReturnToPause()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    // NOTE: ALL KEY REMAPPING LOGIC BELOW THIS LINE IS NOT COMPATIBLE WITH STARTER ASSETS
    // IT IS LEFT HERE FOR YOUR REFERENCE BUT WILL NOT FUNCTION CORRECTLY.

    public void StartRemapForwardKey() { /* Incompatible */ }
    public void StartRemapBackwardKey() { /* Incompatible */ }
    public void StartRemapLeftKey() { /* Incompatible */ }
    public void StartRemapRightKey() { /* Incompatible */ }
    public void StartRemapSprintKey() { /* Incompatible */ }
    public void StartRemapJumpKey() { /* Incompatible */ }
    IEnumerator RemapKey(string keyName, TextMeshProUGUI keyTextUI) { yield break; /* Incompatible */ }
    private void CancelRemap() { /* Incompatible */ }
    public void LoadKeyBindings() { /* Incompatible */ }
}