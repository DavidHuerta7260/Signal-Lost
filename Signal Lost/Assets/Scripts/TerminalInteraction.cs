using UnityEngine;

public class TerminalInteraction : MonoBehaviour
{
    [Header("Settings")]
    public string terminalID = "Terminal_01";
    public string errorMessage = "SYSTEM ERROR 0x0041: Unauthorized access detected.\nInitiating lockdown protocol...";
    public string puzzleSceneName = "PuzzleScene";
    public float displayDuration = 3f;

    [Header("References")]
    public TerminalUI terminalUI;
    public TerminalSceneLoader sceneLoader;
    public GameObject interactPrompt;
    public GameObject completedVisual;

    private bool _playerInRange = false;
    private bool _activated = false;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsTerminalComplete(terminalID))
            SetCompletedState();
    }

    void Update()
    {
        if (_playerInRange && !_activated && Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance != null && GameManager.Instance.IsTerminalComplete(terminalID))
            {
                Debug.Log("Terminal already completed: " + terminalID);
                return;
            }

            Activate();
        }
    }

    void Activate()
    {
        _activated = true;

        // Save player position and which terminal was used
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && GameManager.Instance != null)
            GameManager.Instance.SavePlayerPosition(player.transform.position);

        // Save which puzzle scene to return from
        if (GameManager.Instance != null)
            GameManager.Instance.lastTerminalID = terminalID;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        terminalUI.ShowError(errorMessage);
        sceneLoader.LoadAfterDelay(puzzleSceneName, displayDuration);
    }

    void SetCompletedState()
    {
        _activated = true;

        if (completedVisual != null)
            completedVisual.SetActive(true);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;

            if (interactPrompt != null && GameManager.Instance != null &&
                !GameManager.Instance.IsTerminalComplete(terminalID))
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