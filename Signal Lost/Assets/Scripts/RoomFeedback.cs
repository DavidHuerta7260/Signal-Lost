using UnityEngine;
using System.Collections;

public class RoomFeedback : MonoBehaviour
{
    [Header("Settings")]
    public string requiredTerminalID = "Terminal_01";
    public SpriteRenderer darkOverlay;
    public float fadeDuration = 1.5f;

    void Start()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.IsTerminalComplete(requiredTerminalID))
        {
            // Already completed - instantly remove overlay
            if (darkOverlay != null)
                darkOverlay.color = new Color(0, 0, 0, 0);
        }
        else
        {
            // Not completed - show dark overlay
            if (darkOverlay != null)
                darkOverlay.color = new Color(0, 0, 0, 0.5f);
        }
    }

    public void ActivateFeedback()
    {
        StartCoroutine(FadeOutOverlay());
    }

    IEnumerator FadeOutOverlay()
    {
        if (darkOverlay == null) yield break;

        float elapsed = 0f;
        Color startColor = darkOverlay.color;
        Color endColor = new Color(0, 0, 0, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            darkOverlay.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);
            yield return null;
        }

        darkOverlay.color = endColor;
    }
}
