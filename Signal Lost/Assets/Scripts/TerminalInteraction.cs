using UnityEngine;

public class TerminalInteraction : MonoBehaviour
{
    [Header("Settings")]
    public string errorMessage = "SYSTEM ERROR 0x0041: Unauthorized access detected.\nInitiating lockdown protocol...";
    public string puzzleSceneName = "PuzzleScene";
    public float displayDuration = 3f;

    [Header("References")]
    public TerminalUI terminalUI;           // drag TerminalUI object here
    public TerminalSceneLoader sceneLoader; // drag TerminalSceneLoader here
    public GameObject interactPrompt;       // "Press E" UI element (optional)

    private bool _playerInRange = false;
    private bool _activated = false;

    void Update()
    {
        if (_playerInRange && !_activated && Input.GetKeyDown(KeyCode.E))
        {
            Activate();
        }
    }

    void Activate()
    {
        _activated = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        terminalUI.ShowError(errorMessage);
        sceneLoader.LoadAfterDelay(puzzleSceneName, displayDuration);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}