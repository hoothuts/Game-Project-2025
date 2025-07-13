using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // ADD THIS START METHOD
    void Start()
    {
        Debug.Log("MainMenuController script has initialized and is running.");
    }

    // Public method to be called by the Start Game button
    public void StartGame()
    {
        Debug.Log("Starting Game..."); // Optional: Just to see in Console that it's triggered
        SceneManager.LoadScene("GameScene");
    }

    // Public method to be called by the Options button (for later)
    public void OpenOptions()
    {
        Debug.Log("Opening Options (Not implemented yet)...");
    }

    // Public method to be called by the Exit button
    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}