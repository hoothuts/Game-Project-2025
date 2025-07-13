using UnityEngine;
using UnityEngine.SceneManagement; // For loading scenes
using UnityEngine.UI; // For UI elements (e.g., Button)
using TMPro; // If using TextMeshPro for button text
using System.Collections; // Required for Coroutines

public class GameUIController : MonoBehaviour
{
    public GameObject pausePanel; // Assign your PausePanel GameObject here
    public GameObject optionsPanel; // Assign your OptionsPanel GameObject here

    // --- References for Key Remapping UI Elements (ASSIGN THESE IN INSPECTOR!) ---
    public Button forwardKeyButtonRef; // Drag your 'ForwardKeyButton' GameObject here
    public TextMeshProUGUI forwardKeyText; // Drag the Text (TMP) child of 'ForwardKeyButton' here

    public Button backwardKeyButtonRef; // Drag your 'BackwardKeyButton' GameObject here
    public TextMeshProUGUI backwardKeyText; // Drag the Text (TMP) child of 'BackwardKeyButton' here

    public Button leftKeyButtonRef; // Drag your 'LeftKeyButton' GameObject here
    public TextMeshProUGUI leftKeyText; // Drag the Text (TMP) child of 'LeftKeyButton' here

    public Button rightKeyButtonRef; // Drag your 'RightKeyButton' GameObject here
    public TextMeshProUGUI rightKeyText; // Drag the Text (TMP) child of 'RightKeyButton' here

    public Button sprintKeyButtonRef; // Drag your 'SprintKeyButton' GameObject here
    public TextMeshProUGUI sprintKeyText; // Drag the Text (TMP) child of 'SprintKeyButton' here

    public Button jumpKeyButtonRef; // Drag your 'JumpKeyButton' GameObject here
    public TextMeshProUGUI jumpKeyText; // Drag the Text (TMP) child of 'JumpKeyButton' here
    // --- END Key Remapping UI References ---

    // --- NEW: Reference to InventoryManager (ASSIGN THIS IN INSPECTOR!) ---
    public InventoryManager inventoryManager; // Drag your GameManager/InventorySystemManager GameObject here!


    private bool isGamePaused = false;
    private bool isWaitingForKeyPress = false; // Prevents multiple remapping attempts
    private string keyToRemap; // Stores which key we are currently remapping ("Forward", "Backward", etc.)


    void Start()
    {
        Debug.Log("GameUIController script has initialized and is running."); // Diagnostic log

        // Ensure the pause menu and options panel are hidden when the game starts
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
        ResumeGame(); // Ensure game is unpaused and timeScale is 1

        // --- CODE-BASED BUTTON LINKING FOR REMAP KEYS ---
        // Crucial: You MUST assign these ButtonRefs in the Inspector!
        if (forwardKeyButtonRef != null) forwardKeyButtonRef.onClick.AddListener(StartRemapForwardKey);
        else Debug.LogWarning("ForwardKeyButtonRef not assigned in Inspector. Code-based linking failed.");

        if (backwardKeyButtonRef != null) backwardKeyButtonRef.onClick.AddListener(StartRemapBackwardKey);
        else Debug.LogWarning("BackwardKeyButtonRef not assigned in Inspector. Code-based linking failed.");

        if (leftKeyButtonRef != null) leftKeyButtonRef.onClick.AddListener(StartRemapLeftKey);
        else Debug.LogWarning("LeftKeyButtonRef not assigned in Inspector. Code-based linking failed.");

        if (rightKeyButtonRef != null) rightKeyButtonRef.onClick.AddListener(StartRemapRightKey);
        else Debug.LogWarning("RightKeyButtonRef not assigned in Inspector. Code-based linking failed.");

        if (sprintKeyButtonRef != null) sprintKeyButtonRef.onClick.AddListener(StartRemapSprintKey);
        else Debug.LogWarning("SprintKeyButtonRef not assigned in Inspector. Code-based linking failed.");

        if (jumpKeyButtonRef != null) jumpKeyButtonRef.onClick.AddListener(StartRemapJumpKey);
        else Debug.LogWarning("JumpKeyButtonRef not assigned in Inspector. Code-based linking failed.");
        // --- END CODE-BASED BUTTON LINKING ---

        LoadKeyBindings(); // Load key settings when the game starts
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isWaitingForKeyPress) // If currently remapping, cancel remapping instead of pausing/resuming
            {
                CancelRemap();
            }
            else if (optionsPanel != null && optionsPanel.activeSelf) // If in options, go back to pause menu
            {
                CloseOptionsAndReturnToPause();
            }
            else if (isGamePaused) // If paused, resume
            {
                ResumeGame();
            }
            else // If not paused, pause
            {
                PauseGame();
            }
        }

        // --- NEW: Toggle Inventory with 'I' key ---
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(" 'I' key pressed. Attempting to toggle inventory."); // Diagnostic log
            if (inventoryManager != null)
            {
                inventoryManager.ToggleInventoryUI();
            }
            else
            {
                Debug.LogError("InventoryManager reference is NULL in GameUIController! Please assign the GameManager/InventorySystemManager GameObject in the Inspector."); // Error log if not assigned
            }
        }
        // --- END NEW ---
    }

    public void PauseGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true); // Show the pause menu
        }
        Time.timeScale = 0f; // Stop time (pauses all physics, animations, and Update loops)
        isGamePaused = true;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // Hide the pause menu
        }
        if (optionsPanel != null) // Also hide options if they were open
        {
            optionsPanel.SetActive(false);
        }
        Time.timeScale = 1f; // Resume normal time
        isGamePaused = false;
        Debug.Log("Game Resumed");
    }

    public void OpenOptionsFromPause()
    {
        Debug.Log("Opening Options from Pause Menu...");
        // Hide pause menu and show options menu
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        // Time.timeScale remains 0f while in options
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");
        Time.timeScale = 1f; // IMPORTANT: Reset timeScale before loading new scene
        SceneManager.LoadScene("MainMenu"); // Load your main menu scene
    }

    // Method for the "Back" button in your options menu
    public void CloseOptionsAndReturnToPause()
    {
        Debug.Log("Closing Options, returning to Pause Menu...");
        isWaitingForKeyPress = false; // Ensure remapping is cancelled if active
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    // --- KEY REMAPPING LOGIC ---

    // Public methods to be called by each key remapping button
    public void StartRemapForwardKey() { StartCoroutine(RemapKey("Forward", forwardKeyText)); }
    public void StartRemapBackwardKey() { StartCoroutine(RemapKey("Backward", backwardKeyText)); }
    public void StartRemapLeftKey() { StartCoroutine(RemapKey("Left", leftKeyText)); }
    public void StartRemapRightKey() { StartCoroutine(RemapKey("Right", rightKeyText)); }
    public void StartRemapSprintKey() { StartCoroutine(RemapKey("Sprint", sprintKeyText)); }
    public void StartRemapJumpKey() { StartCoroutine(RemapKey("Jump", jumpKeyText)); }


    // The main coroutine that handles detecting a new key press
    IEnumerator RemapKey(string keyName, TextMeshProUGUI keyTextUI)
    {
        Debug.Log($"Starting RemapKey Coroutine for: {keyName}. isWaitingForKeyPress: {isWaitingForKeyPress}");

        if (isWaitingForKeyPress)
        {
            Debug.LogWarning("Already waiting for a key press. Cancelling previous remapping.");
            StopAllCoroutines();
            isWaitingForKeyPress = false;
        }

        isWaitingForKeyPress = true;
        keyToRemap = keyName;

        string originalKeyText = keyTextUI != null ? keyTextUI.text : "ERROR";
        if (keyTextUI != null)
        {
            keyTextUI.text = "Press a key...";
        }
        else
        {
            Debug.LogError($"RemapKey: keyTextUI for {keyName} is NULL. Make sure it's assigned in Inspector.");
            isWaitingForKeyPress = false;
            yield break;
        }

        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        Debug.Log("Input.anyKeyDown detected. Attempting to find key...");

        KeyCode detectedKeyCode = KeyCode.None;
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (keyCode == KeyCode.Escape || keyCode == KeyCode.Menu ||
                keyCode == KeyCode.LeftAlt || keyCode == KeyCode.RightAlt ||
                keyCode == KeyCode.LeftControl || keyCode == KeyCode.RightControl ||
                keyCode == KeyCode.LeftShift || keyCode == KeyCode.RightShift ||
                keyCode == KeyCode.Mouse0 || keyCode == KeyCode.Mouse1 || keyCode == KeyCode.Mouse2)
            {
                continue;
            }

            if (Input.GetKeyDown(keyCode))
            {
                detectedKeyCode = keyCode;
                break;
            }
        }

        if (detectedKeyCode != KeyCode.None)
        {
            Debug.Log($"Key detected: {detectedKeyCode}. Saving to PlayerPrefs.");
            PlayerPrefs.SetString(keyName + "Key", detectedKeyCode.ToString());
            PlayerPrefs.Save();

            keyTextUI.text = detectedKeyCode.ToString();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    Debug.Log($"Applying new key to PlayerMovement: {keyName} = {detectedKeyCode}");
                    switch (keyName)
                    {
                        case "Forward": playerMovement.moveForwardKey = detectedKeyCode; break;
                        case "Backward": playerMovement.moveBackwardKey = detectedKeyCode; break;
                        case "Left": playerMovement.moveLeftKey = detectedKeyCode; break;
                        case "Right": playerMovement.moveRightKey = detectedKeyCode; break;
                        case "Sprint": playerMovement.sprintKey = detectedKeyCode; break;
                        case "Jump": playerMovement.jumpKey = detectedKeyCode; break;
                    }
                }
                else
                {
                    Debug.LogError("PlayerMovement component not found on Player GameObject. Make sure it's attached.");
                }
            }
            else
            {
                Debug.LogError("Player GameObject with 'Player' tag not found. Make sure your player has the tag.");
            }
        }
        else
        {
            Debug.LogWarning("No valid key detected after click. Remapping cancelled.");
            if (keyTextUI != null)
            {
                keyTextUI.text = originalKeyText;
            }
        }

        isWaitingForKeyPress = false;
        Debug.Log("RemapKey Coroutine finished.");
    }

    private void CancelRemap()
    {
        Debug.Log("Remapping cancelled by user or new action.");
        StopAllCoroutines();
        isWaitingForKeyPress = false;
        LoadKeyBindings();
    }


    // --- LOADING KEY BINDINGS ---
    public void LoadKeyBindings()
    {
        Debug.Log("Loading Key Bindings...");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("LoadKeyBindings: Player GameObject with 'Player' tag not found. Cannot load key bindings.");
            return;
        }
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("LoadKeyBindings: PlayerMovement component not found on Player GameObject. Cannot load key bindings.");
            return;
        }

        void LoadAndApplyKey(string keyName, KeyCode defaultKey, ref KeyCode playerMoveKey, TextMeshProUGUI uiText)
        {
            string keyString = PlayerPrefs.GetString(keyName + "Key", defaultKey.ToString());
            KeyCode loadedKey = KeyCode.None;
            try
            {
                loadedKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyString);
            }
            catch (System.ArgumentException)
            {
                Debug.LogWarning($"PlayerPrefs: Invalid KeyCode string '{keyString}' for {keyName}. Using default.");
                loadedKey = defaultKey;
                PlayerPrefs.SetString(keyName + "Key", defaultKey.ToString());
                PlayerPrefs.Save();
            }

            playerMoveKey = loadedKey;
            if (uiText != null) uiText.text = loadedKey.ToString();
            Debug.Log($"Loaded {keyName} Key: {loadedKey}");
        }

        LoadAndApplyKey("Forward", KeyCode.W, ref playerMovement.moveForwardKey, forwardKeyText);
        LoadAndApplyKey("Backward", KeyCode.S, ref playerMovement.moveBackwardKey, backwardKeyText);
        LoadAndApplyKey("Left", KeyCode.A, ref playerMovement.moveLeftKey, leftKeyText);
        LoadAndApplyKey("Right", KeyCode.D, ref playerMovement.moveRightKey, rightKeyText);
        LoadAndApplyKey("Sprint", KeyCode.LeftShift, ref playerMovement.sprintKey, sprintKeyText);
        LoadAndApplyKey("Jump", KeyCode.Space, ref playerMovement.jumpKey, jumpKeyText);
    }
}