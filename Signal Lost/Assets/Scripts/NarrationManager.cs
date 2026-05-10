using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NarrationManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject narrationPanel;
    public TextMeshProUGUI narrationText;
    public TextMeshProUGUI skipText;

    [Header("Settings")]
    public float typeSpeed = 0.04f;
    public float linePause = 0.8f;

    [Header("Narration Lines")]
    [TextArea(2, 4)]
    public string[] narrationLines;

    private bool isTyping = false;
    private bool skipRequested = false;
    private bool narrationComplete = false;

    void Start()
    {
        // Only show narration on first load
        if (!GameManager.hasSavedPosition)
        {
            narrationPanel.SetActive(true);
            Time.timeScale = 0f;
            StartCoroutine(PlayNarration());
        }
        else
        {
            narrationPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (narrationPanel.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                // Skip current typing
                skipRequested = true;
            }
            else if (narrationComplete)
            {
                // Close panel
                CloseNarration();
            }
        }
    }

    IEnumerator PlayNarration()
    {
        narrationText.text = "";

        foreach (string line in narrationLines)
        {
            skipRequested = false;
            isTyping = true;

            // Type each character
            foreach (char c in line)
            {
                if (skipRequested)
                {
                    narrationText.text += line.Substring(narrationText.text.Length - GetCurrentLineLength(narrationText.text, line));
                    break;
                }

                narrationText.text += c;
                yield return new WaitForSecondsRealtime(typeSpeed);
            }

            isTyping = false;

            // Pause between lines
            yield return new WaitForSecondsRealtime(linePause);

            // Add new line
            narrationText.text += "\n";
        }

        narrationComplete = true;

        // Blink skip text to show player can close
        if (skipText != null)
            skipText.text = "Press E to Continue";

        StartCoroutine(BlinkSkipText());
    }

    int GetCurrentLineLength(string fullText, string line)
    {
        int index = fullText.LastIndexOf(line[0]);
        if (index < 0) return 0;
        return fullText.Length - index;
    }

    IEnumerator BlinkSkipText()
    {
        while (!narrationComplete || narrationPanel.activeSelf)
        {
            if (skipText != null)
                skipText.enabled = !skipText.enabled;
            yield return new WaitForSecondsRealtime(0.6f);
        }
    }

    void CloseNarration()
    {
        narrationPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}