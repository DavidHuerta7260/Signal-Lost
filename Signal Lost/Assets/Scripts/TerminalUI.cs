using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerminalUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject errorPanel;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI cursorText;

    [Header("Typewriter Settings")]
    public float typeSpeed = 0.04f;

    // Colors
    private Color panelColor = new Color(0.02f, 0.05f, 0.1f, 0.95f);
    private Color borderColor = new Color(0f, 0.9f, 1f, 1f);
    private Color textColor = new Color(0f, 0.9f, 1f, 1f);

    void Awake()
    {
        if (errorPanel != null)
        {
            BuildStyling();
            errorPanel.SetActive(false);
        }
    }

    void BuildStyling()
    {
        // Panel background
        Image panelImage = errorPanel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = panelColor;

        // Style error text
        if (errorText != null)
        {
            errorText.color = textColor;
            errorText.fontSize = 30;
            errorText.alignment = TextAlignmentOptions.TopLeft;
            errorText.enableWordWrapping = true;
            errorText.lineSpacing = 8;

            RectTransform errorRect = errorText.GetComponent<RectTransform>();
            errorRect.anchorMin = new Vector2(0f, 0f);
            errorRect.anchorMax = new Vector2(1f, 1f);
            errorRect.offsetMin = new Vector2(25f, 50f);
            errorRect.offsetMax = new Vector2(-25f, -55f);
        }

        // Style cursor text
        if (cursorText != null)
        {
            cursorText.color = textColor;
            cursorText.fontSize = 17;

            RectTransform cursorRect = cursorText.GetComponent<RectTransform>();
            cursorRect.anchorMin = new Vector2(0f, 0f);
            cursorRect.anchorMax = new Vector2(1f, 0f);
            cursorRect.offsetMin = new Vector2(20f, 10f);
            cursorRect.offsetMax = new Vector2(-20f, 35f);
        }

        // Border Top
        CreateBorder("BorderTop", errorPanel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, -3f), new Vector2(0f, 0f));

        // Border Bottom
        CreateBorder("BorderBottom", errorPanel.transform,
            new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 0f), new Vector2(0f, 3f));

        // Border Left
        CreateBorder("BorderLeft", errorPanel.transform,
            new Vector2(0f, 0f), new Vector2(0f, 1f),
            new Vector2(0f, 0f), new Vector2(3f, 0f));

        // Border Right
        CreateBorder("BorderRight", errorPanel.transform,
            new Vector2(1f, 0f), new Vector2(1f, 1f),
            new Vector2(-3f, 0f), new Vector2(0f, 0f));

        // Header divider line
        CreateBorder("HeaderDivider", errorPanel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(15f, -42f), new Vector2(-15f, -39f));

        // Header text
        TextMeshProUGUI header = CreateText("HeaderText", errorPanel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(20f, -35f), new Vector2(-20f, 0f));
        header.text = "// SYSTEM ERROR DETECTED";
        header.fontSize = 13;
        header.fontStyle = FontStyles.Bold;
        header.color = new Color(1f, 1f, 1f, 0.8f);
        header.alignment = TextAlignmentOptions.Left;

        // Corner decorations
        CreateCorner("CornerTL", errorPanel.transform, new Vector2(0f, 1f), new Vector2(8f, -8f));
        CreateCorner("CornerTR", errorPanel.transform, new Vector2(1f, 1f), new Vector2(-8f, -8f));
        CreateCorner("CornerBL", errorPanel.transform, new Vector2(0f, 0f), new Vector2(8f, 8f));
        CreateCorner("CornerBR", errorPanel.transform, new Vector2(1f, 0f), new Vector2(-8f, 8f));
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

    public void ShowError(string message)
    {
        errorPanel.SetActive(true);
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