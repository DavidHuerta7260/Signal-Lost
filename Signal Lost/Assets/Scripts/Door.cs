using UnityEngine;
using TMPro;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    public string requiredTerminalID = "Terminal_01";
    public bool isLocked = true;

    [Header("References")]
    public BoxCollider2D blockingCollider;

    [Header("UI")]
    public GameObject lockedPanel;
    public TextMeshProUGUI lockedText;
    public GameObject unlockPromptPanel;
    public TextMeshProUGUI unlockPromptText;
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    [Header("Messages")]
    public string lockedMessage = "You need to complete Terminal {0} to continue.";
    public string unlockPromptMessage = "Terminal complete!\nPress E to unlock the door.";
    public string unlockedMessage = "ACCESS GRANTED\nDoor unlocked.";

    [Header("Timing")]
    public float messageDuration = 2f;
    public float doorDisappearDelay = 1.5f;

    private bool playerInRange = false;
    private bool waitingForUnlock = false;

    void Start()
    {
        if (lockedPanel != null)
            lockedPanel.SetActive(false);

        if (unlockPromptPanel != null)
            unlockPromptPanel.SetActive(false);

        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (waitingForUnlock)
            {
                // Player pressed E to confirm unlock
                StartCoroutine(UnlockSequence());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (GameManager.Instance != null &&
            GameManager.Instance.IsTerminalComplete(requiredTerminalID))
        {
            // Terminal complete - show unlock prompt
            ShowUnlockPrompt();
        }
        else
        {
            // Terminal not complete - show locked message
            ShowLockedMessage();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        waitingForUnlock = false;

        HideAllPanels();
    }

    void ShowLockedMessage()
    {
        if (lockedPanel != null && lockedText != null)
        {
            lockedText.text = string.Format(lockedMessage, requiredTerminalID);
            lockedPanel.SetActive(true);
        }
    }

    void ShowUnlockPrompt()
    {
        waitingForUnlock = true;

        if (unlockPromptPanel != null && unlockPromptText != null)
        {
            unlockPromptText.text = unlockPromptMessage;
            unlockPromptPanel.SetActive(true);
        }
    }

    void HideAllPanels()
    {
        if (lockedPanel != null)
            lockedPanel.SetActive(false);

        if (unlockPromptPanel != null)
            unlockPromptPanel.SetActive(false);

        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    IEnumerator UnlockSequence()
    {
        isLocked = false;
        waitingForUnlock = false;

        if (blockingCollider != null)
            blockingCollider.enabled = false;

        HideAllPanels();

        // Show access granted message
        if (messagePanel != null && messageText != null)
        {
            messageText.text = unlockedMessage;
            messagePanel.SetActive(true);

            yield return new WaitForSeconds(messageDuration);

            messagePanel.SetActive(false);
        }

        yield return new WaitForSeconds(doorDisappearDelay);

        gameObject.SetActive(false);
    }
}