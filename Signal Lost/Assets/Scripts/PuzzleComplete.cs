using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PuzzleComplete : MonoBehaviour
{
    [Header("UI")]
    public GameObject completionPanel;
    public TextMeshProUGUI congratsText;
    public Button continueButton;

    [Header("Settings")]
    public string mainLevelScene = "MainLevel";
    public string terminalID = "Terminal_01";
    public string congratsMessage = "SYSTEM RESTORED";
    public float panelDelay = 1f;

    private bool isPuzzleComplete = false;

    void Start()
    {
        if (completionPanel != null)
            completionPanel.SetActive(false);

        if (congratsText != null)
            congratsText.text = congratsMessage;

        if (continueButton != null)
            continueButton.onClick.AddListener(ReturnToMainLevel);
    }

    public void CompletePuzzle()
    {
        if (isPuzzleComplete) return;
        isPuzzleComplete = true;

        if (GameManager.Instance != null)
            GameManager.Instance.CompleteTerminal(terminalID);

        Invoke("ShowPanel", panelDelay);
    }

    void ShowPanel()
    {
        if (completionPanel != null)
            completionPanel.SetActive(true);
    }

    void ReturnToMainLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainLevelScene);
    }
}