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

    // Colors
    private Color panelColor = new Color(0.02f, 0.05f, 0.1f, 0.95f);
    private Color borderColor = new Color(0f, 0.9f, 1f, 1f);
    private Color textColor = new Color(0f, 0.9f, 1f, 1f);
    private Color skipColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    private bool isTyping = false;
    private bool skipRequested = false;
    private bool narrationComplete = false;

    void Awake()
    {
        BuildStyling();
    }

    void BuildStyling()
    {
        // Style the panel background
        Image panelImage = narrationPanel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = panelColor;

        // Style narration text
        if (narrationText != null)
        {
            narrationText.color = textColor;
            narrationText.fontSize = 17;
            narrationText.alignment = TextAlignmentOptions.TopLeft;
            narrationText.enableWordWrapping = true;
            narrationText.lineSpacing = 8;

            // Force narration text to fill the panel
            RectTransform narrationRect = narrationText.GetComponent<RectTransform>();
            narrationRect.anchorMin = new Vector2(0f, 0f);
            narrationRect.anchorMax = new Vector2(1f, 1f);
            narrationRect.offsetMin = new Vector2(25f, 50f);
            narrationRect.offsetMax = new Vector2(-25f, -55f);
        }

        // Style skip text
        if (skipText != null)
        {
            skipText.text = "[ E ] Skip";
            skipText.color = skipColor;
            skipText.fontSize = 12;
            skipText.alignment = TextAlignmentOptions.Right;

            // Force skip text to bottom right
            RectTransform skipRect = skipText.GetComponent<RectTransform>();
            skipRect.anchorMin = new Vector2(0f, 0f);
            skipRect.anchorMax = new Vector2(1f, 0f);
            skipRect.offsetMin = new Vector2(20f, 10f);
            skipRect.offsetMax = new Vector2(-20f, 35f);
        }

        // Border Top
        CreateBorder("BorderTop", narrationPanel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, -3f), new Vector2(0f, 0f));

        // Border Bottom
        CreateBorder("BorderBottom", narrationPanel.transform,
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 0f), new Vector2(0f, 3f));

        // Border Left
        CreateBorder("BorderLeft", narrationPanel.transform,
            new Vector2(0f, 0f), new Vector2(0f, 1f),
            new Vector2(0f, 0f), new Vector2(3f, 0f));

        // Border Right
        CreateBorder("BorderRight", narrationPanel.transform,
            new Vector2(1f, 0f), new Vector2(1f, 1f),
            new Vector2(-3f, 0f), new Vector2(0f, 0f));

        // Header divider line
        CreateBorder("HeaderDivider", narrationPanel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(15f, -42f), new Vector2(-15f, -39f));

        // Header text
        TextMeshProUGUI header = CreateText("HeaderText", narrationPanel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(20f, -35f), new Vector2(-20f, 0f));
        header.text = "// INCOMING TRANSMISSION";
        header.fontSize = 13;
        header.fontStyle = FontStyles.Bold;
        header.color = new Color(1f, 1f, 1f, 0.8f);
        header.alignment = TextAlignmentOptions.Left;

        // Corner decorations
        CreateCorner("CornerTL", narrationPanel.transform, new Vector2(0f, 1f), new Vector2(8f, -8f));
        CreateCorner("CornerTR", narrationPanel.transform, new Vector2(1f, 1f), new Vector2(-8f, -8f));
        CreateCorner("CornerBL", narrationPanel.transform, new Vector2(0f, 0f), new Vector2(8f, 8f));
        CreateCorner("CornerBR", narrationPanel.transform, new Vector2(1f, 0f), new Vector2(-8f, 8f));
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

    void Start()
    {
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
                skipRequested = true;
            else if (narrationComplete)
                CloseNarration();
        }
    }

    IEnumerator PlayNarration()
    {
        narrationText.text = "";

        foreach (string line in narrationLines)
        {
            skipRequested = false;
            isTyping = true;

            foreach (char c in line)
            {
                if (skipRequested)
                {
                    narrationText.text += line.Substring(
                        narrationText.text.Length - GetCurrentLineLength(narrationText.text, line));
                    break;
                }

                narrationText.text += c;
                yield return new WaitForSecondsRealtime(typeSpeed);
            }

            isTyping = false;
            yield return new WaitForSecondsRealtime(linePause);
            narrationText.text += "\n";
        }

        narrationComplete = true;

        if (skipText != null)
            skipText.text = "[ E ] Continue";

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
        while (narrationPanel.activeSelf)
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