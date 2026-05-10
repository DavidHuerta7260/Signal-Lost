using UnityEngine;
using TMPro;

public class DoorPrompt : MonoBehaviour
{
    [Header("References")]
    public Door door;
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;

    [Header("Settings")]
    public string lockedMessage = "SYSTEM OFFLINE\nComplete Terminal {0} to unlock";

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && door.isLocked && Input.GetKeyDown(KeyCode.E))
            ShowLockedMessage();
    }

    void ShowLockedMessage()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(true);
            if (promptText != null)
                promptText.text = string.Format(lockedMessage, door.requiredTerminalID);

            Invoke("HidePrompt", 2f);
        }
    }

    void HidePrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
        }
    }
}
