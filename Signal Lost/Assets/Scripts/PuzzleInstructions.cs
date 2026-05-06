using UnityEngine;
using TMPro;
using System.Collections;

public class PuzzleInstructions : MonoBehaviour
{
    [Header("UI References")]
    public GameObject instructionPanel;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI continueText;

    [Header("Instructions")]
    [TextArea(3, 6)]
    public string[] instructionPages;

    private int currentPage = 0;
    private bool canAdvance = false;
    private bool isComplete = false;

    void Start()
    {
        if (instructionPages.Length > 0)
        {
            instructionPanel.SetActive(true);
            ShowPage(0);
            StartCoroutine(BlinkContinueText());

            // Freeze time while instructions are open
            Time.timeScale = 0f;
        }
        else
        {
            instructionPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isComplete) return;

        if (canAdvance && Input.GetKeyDown(KeyCode.E))
        {
            if (currentPage < instructionPages.Length - 1)
            {
                currentPage++;
                ShowPage(currentPage);
            }
            else
            {
                CloseInstructions();
            }
        }
    }

    void ShowPage(int index)
    {
        canAdvance = false;
        instructionText.text = instructionPages[index];

        // Update continue text based on page
        if (index == instructionPages.Length - 1)
            continueText.text = "Press E to Begin";
        else
            continueText.text = "Press E to Continue";

        StartCoroutine(EnableAdvanceAfterDelay());
    }

    IEnumerator EnableAdvanceAfterDelay()
    {
        // Use WaitForSecondsRealtime since timeScale is 0
        yield return new WaitForSecondsRealtime(0.5f);
        canAdvance = true;
    }

    IEnumerator BlinkContinueText()
    {
        while (!isComplete)
        {
            if (continueText != null)
                continueText.enabled = !continueText.enabled;
            yield return new WaitForSecondsRealtime(0.6f);
        }
    }

    void CloseInstructions()
    {
        isComplete = true;
        Time.timeScale = 1f;
        instructionPanel.SetActive(false);
    }
}
