// IntroController.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI introTextUI;

    [Header("Scene & Text Settings")]
    public string nextSceneName = "MainMenu";
    public float typingSpeed = 0.05f; // Time between each character

    [Header("Intro Dialogue")]
    [TextArea(3, 10)] // Makes the text box in the Inspector bigger
    public string[] introLines;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        // Make sure there are lines to display before starting
        if (introLines != null && introLines.Length > 0)
        {
            StartTypingNextLine();
        }
        else
        {
            Debug.LogWarning("No intro lines have been set in the IntroController.");
        }
    }

    void Update()
    {
        // Listen for the Enter key
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            HandleEnterPress();
        }
    }

    private void HandleEnterPress()
    {
        // If a line is currently being typed, skip the animation
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            introTextUI.text = introLines[currentLineIndex];
            isTyping = false;
        }
        // If the line is finished, move to the next one
        else
        {
            currentLineIndex++;
            if (currentLineIndex < introLines.Length)
            {
                StartTypingNextLine();
            }
            else
            {
                // All lines have been shown, load the main menu
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void StartTypingNextLine()
    {
        typingCoroutine = StartCoroutine(TypeLine(introLines[currentLineIndex]));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        introTextUI.text = ""; // Clear the text field
        foreach (char letter in line.ToCharArray())
        {
            introTextUI.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
}