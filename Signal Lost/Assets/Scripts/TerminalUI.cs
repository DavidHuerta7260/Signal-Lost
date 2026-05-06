using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerminalUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject errorPanel;       // the background panel
    public TextMeshProUGUI errorText;   // the message text
    public TextMeshProUGUI cursorText;  // blinking cursor (optional, set text to "_")

    [Header("Typewriter Settings")]
    public float typeSpeed = 0.04f;     // seconds per character
    public Color errorColor = Color.white; // classic green terminal

    void Awake()
    {
        if (errorPanel != null)
            errorPanel.SetActive(false);
    }

    public void ShowError(string message)
    {
        errorPanel.SetActive(true);
        errorText.color = errorColor;
        StartCoroutine(TypewriterEffect(message));

        if (cursorText != null)
            StartCoroutine(BlinkCursor());
    }

    IEnumerator TypewriterEffect(string message)
    {
        errorText.text = "";
        foreach (char c in message)
        {
            errorText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator BlinkCursor()
    {
        while (true)
        {
            cursorText.enabled = !cursorText.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }
}