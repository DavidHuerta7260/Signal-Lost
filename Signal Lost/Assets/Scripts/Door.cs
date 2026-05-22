using UnityEngine;
using UnityEngine.UI;
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

    // Colors
    private Color panelColor = new Color(0.06f, 0.18f, 0.38f, 0.95f);
    private Color borderColor = new Color(0f, 0.9f, 1f, 1f);
    private Color textColor = new Color(0f, 0.9f, 1f, 1f);
    private Color headerColor = new Color(1f, 1f, 1f, 0.8f);

    private bool playerInRange = false;
    private bool waitingForUnlock = false;

    void Start()
    {
        if (lockedPanel != null)
        {
            ApplyPanelStyling(lockedPanel, lockedText, "// ACCESS DENIED");
            lockedPanel.SetActive(false);
        }

        if (unlockPromptPanel != null)
        {
            ApplyPanelStyling(unlockPromptPanel, unlockPromptText, "// TERMINAL RESTORED");
            unlockPromptPanel.SetActive(false);
        }

        if (messagePanel != null)
        {
            ApplyPanelStyling(messagePanel, messageText, "// SYSTEM STATUS");
            messagePanel.SetActive(false);
        }
    }

    void ApplyPanelStyling(GameObject panel, TextMeshProUGUI text, string headerLabel)
    {
        // Panel background
        Image panelImage = panel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = panelColor;

        // Main text styling
        if (text != null)
        {
            text.color = textColor;
            text.fontSize = 30;
            text.alignment = TextAlignmentOptions.Center;
            text.enableWordWrapping = true;
        }

        // Borders
        CreateBorder("BorderTop", panel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, -3f), new Vector2(0f, 0f));
        CreateBorder("BorderBottom", panel.transform,
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 0f), new Vector2(0f, 3f));
        CreateBorder("BorderLeft", panel.transform,
            new Vector2(0f, 0f), new Vector2(0f, 1f),
            new Vector2(0f, 0f), new Vector2(3f, 0f));
        CreateBorder("BorderRight", panel.transform,
            new Vector2(1f, 0f), new Vector2(1f, 1f),
            new Vector2(-3f, 0f), new Vector2(0f, 0f));

        // Header divider
        CreateBorder("HeaderDivider", panel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(15f, -42f), new Vector2(-15f, -39f));

        // Header text
        TextMeshProUGUI header = CreateText("HeaderText", panel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(20f, -35f), new Vector2(-20f, 0f));
        header.text = headerLabel;
        header.fontSize = 13;
        header.fontStyle = FontStyles.Bold;
        header.color = headerColor;
        header.alignment = TextAlignmentOptions.Left;

        // Corner decorations
        CreateCorner("CornerTL", panel.transform, new Vector2(0f, 1f), new Vector2(8f, -8f));
        CreateCorner("CornerTR", panel.transform, new Vector2(1f, 1f), new Vector2(-8f, -8f));
        CreateCorner("CornerBL", panel.transform, new Vector2(0f, 0f), new Vector2(8f, 8f));
        CreateCorner("CornerBR", panel.transform, new Vector2(1f, 0f), new Vector2(-8f, 8f));
    }

    void CreateBorder(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject border = new GameObject(name);
        border.transform.SetParent(parent, false);

        RectTransform rect = border.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;

        Image img = border.AddComponent<Image>();
        img.color = borderColor;
    }

    void CreateCorner(string name, Transform parent, Vector2 anchor, Vector2 offset)
    {
        GameObject corner = new GameObject(name);
        corner.transform.SetParent(parent, false);

        RectTransform rect = corner.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(12f, 12f);
        rect.anchoredPosition = offset;

        Image img = corner.AddComponent<Image>();
        img.color = borderColor;
    }

    TextMeshProUGUI CreateText(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;

        return obj.AddComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (waitingForUnlock)
                StartCoroutine(UnlockSequence());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (GameManager.Instance != null &&
            GameManager.Instance.IsTerminalComplete(requiredTerminalID))
            ShowUnlockPrompt();
        else
            ShowLockedMessage();
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
        if (lockedPanel != null) lockedPanel.SetActive(false);
        if (unlockPromptPanel != null) unlockPromptPanel.SetActive(false);
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    IEnumerator UnlockSequence()
    {
        isLocked = false;
        waitingForUnlock = false;

        if (blockingCollider != null)
            blockingCollider.enabled = false;

        HideAllPanels();

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