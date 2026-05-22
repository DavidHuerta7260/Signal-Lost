using UnityEngine;
using UnityEngine.UI;
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

    // Colors
    private Color panelColor = new Color(0.02f, 0.05f, 0.1f, 0.95f);
    private Color borderColor = new Color(0f, 0.9f, 1f, 1f);
    private Color textColor = new Color(0f, 0.9f, 1f, 1f);
    private Color continueColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    void Awake()
    {
        BuildStyling();
    }

    void BuildStyling()
    {
        // Panel background
        Image panelImage = instructionPanel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = panelColor;

        // Style instruction text
        if (instructionText != null)
        {
            instructionText.color = textColor;
            instructionText.fontSize = 30;
            instructionText.alignment = TextAlignmentOptions.TopLeft;
            instructionText.enableWordWrapping = true;
            instructionText.lineSpacing = 8;

            RectTransform instructionRect = instructionText.GetComponent<RectTransform>();
            instructionRect.anchorMin = new Vector2(0f, 0f);
            instructionRect.anchorMax = new Vector2(1f, 1f);
            instructionRect.offsetMin = new Vector2(25f, 50f);
            instructionRect.offsetMax = new Vector2(-25f, -55f);
        }

        // Style continue text
        if (continueText != null)
        {
            continueText.color = continueColor;
            continueText.fontSize = 25;
            continueText.alignment = TextAlignmentOptions.Right;

            RectTransform continueRect = continueText.GetComponent<RectTransform>();
            continueRect.anchorMin = new Vector2(0f, 0f);
            continueRect.anchorMax = new Vector2(1f, 0f);
            continueRect.offsetMin = new Vector2(20f, 10f);
            continueRect.offsetMax = new Vector2(-20f, 35f);
        }

        // Border Top
        CreateBorder("BorderTop", instructionPanel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, -3f), new Vector2(0f, 0f));

        // Border Bottom
        CreateBorder("BorderBottom", instructionPanel.transform,
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 0f), new Vector2(0f, 3f));

        // Border Left
        CreateBorder("BorderLeft", instructionPanel.transform,
            new Vector2(0f, 0f), new Vector2(0f, 1f),
            new Vector2(0f, 0f), new Vector2(3f, 0f));

        // Border Right
        CreateBorder("BorderRight", instructionPanel.transform,
            new Vector2(1f, 0f), new Vector2(1f, 1f),
            new Vector2(-3f, 0f), new Vector2(0f, 0f));

        // Header divider line
        CreateBorder("HeaderDivider", instructionPanel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(15f, -42f), new Vector2(-15f, -39f));

        // Page indicator divider line
        CreateBorder("FooterDivider", instructionPanel.transform,
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(15f, 38f), new Vector2(-15f, 41f));

        // Header text
        TextMeshProUGUI header = CreateText("HeaderText", instructionPanel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(20f, -35f), new Vector2(-20f, 0f));
        header.text = "// MISSION BRIEFING";
        header.fontSize = 13;
        header.fontStyle = FontStyles.Bold;
        header.color = new Color(1f, 1f, 1f, 0.8f);
        header.alignment = TextAlignmentOptions.Left;

        // Corner decorations
        CreateCorner("CornerTL", instructionPanel.transform, new Vector2(0f, 1f), new Vector2(8f, -8f));
        CreateCorner("CornerTR", instructionPanel.transform, new Vector2(1f, 1f), new Vector2(-8f, -8f));
        CreateCorner("CornerBL", instructionPanel.transform, new Vector2(0f, 0f), new Vector2(8f, 8f));
        CreateCorner("CornerBR", instructionPanel.transform, new Vector2(1f, 0f), new Vector2(-8f, 8f));
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
        if (instructionPages.Length > 0)
        {
            instructionPanel.SetActive(true);
            ShowPage(0);
            StartCoroutine(BlinkContinueText());
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

        if (index == instructionPages.Length - 1)
            continueText.text = "[ E ] Begin";
        else
            continueText.text = "[ E ] Continue";

        StartCoroutine(EnableAdvanceAfterDelay());
    }

    IEnumerator EnableAdvanceAfterDelay()
    {
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