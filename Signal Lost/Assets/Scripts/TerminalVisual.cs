using UnityEngine;

public class TerminalVisual : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Check if already completed when scene loads
        TerminalInteraction terminal = GetComponent<TerminalInteraction>();
        if (terminal != null && GameManager.Instance != null &&
            GameManager.Instance.IsTerminalComplete(terminal.terminalID))
        {
            SetActive();
        }
        else
        {
            SetInactive();
        }
    }

    public void SetActive()
    {
        if (sr != null && activeSprite != null)
            sr.sprite = activeSprite;
    }

    public void SetInactive()
    {
        if (sr != null && inactiveSprite != null)
            sr.sprite = inactiveSprite;
    }
}
