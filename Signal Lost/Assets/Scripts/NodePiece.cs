using UnityEngine;

public class NodePiece : MonoBehaviour
{
    [Header("Connection")]
    public bool up;
    public bool down;
    public bool left;
    public bool right;

    public void SetActive(bool active)
    {
        // No visual change needed for node
    }

    public bool GetConnection(string direction)
    {
        switch (direction)
        {
            case "up": return up;
            case "down": return down;
            case "left": return left;
            case "right": return right;
            default: return false;
        }
    }
}