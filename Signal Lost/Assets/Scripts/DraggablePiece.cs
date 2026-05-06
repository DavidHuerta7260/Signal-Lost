using UnityEngine;

public class DraggablePiece : MonoBehaviour
{
    [Header("Pipe Connections for this piece shape")]
    public bool up;
    public bool down;
    public bool left;
    public bool right;

    private Vector3 offset;
    private bool isDragging = false;
    private Transform originalParent;
    private Vector3 originalPosition;

    void OnMouseDown()
    {
        string parentName = transform.parent != null ? transform.parent.name : "none";
        string parentTag = transform.parent != null ? transform.parent.tag : "none";
        Debug.Log("Clicked: " + gameObject.name + " parent: " + parentName + " tag: " + parentTag);

        if (transform.parent != null && transform.parent.CompareTag("Slot"))
        {
            PuzzlePiece slot = transform.parent.GetComponent<PuzzlePiece>();
            if (slot != null)
            {
                slot.Rotate();
                return;
            }
        }

        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
        originalParent = transform.parent;
        originalPosition = transform.position;
        transform.SetParent(null);
    }

    void OnMouseDrag()
    {
        if (isDragging)
            transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        int slotLayer = LayerMask.GetMask("Slot");
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position, slotLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Slot"))
            {
                transform.position = hit.bounds.center;
                transform.SetParent(hit.transform);

                PuzzlePiece slot = hit.GetComponent<PuzzlePiece>();
                if (slot != null)
                {
                    slot.up = up;
                    slot.down = down;
                    slot.left = left;
                    slot.right = right;
                    slot.rotationState = 0;
                    slot.ResetRotation();
                }

                // Refresh animator now that pipe is parented to slot
                PipePiece pipePiece = hit.GetComponent<PipePiece>();
                if (pipePiece != null)
                    pipePiece.RefreshAnimator();

                FindObjectOfType<PuzzleManager>().ActivateSystem();
                return;
            }
        }

        transform.SetParent(originalParent);
        transform.position = originalPosition;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
}