using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public int rotationState = 0;

    public bool up;
    public bool down;
    public bool left;
    public bool right;

    private Transform GetPipeVisual()
    {
        Animator childAnim = GetComponentInChildren<Animator>();
        if (childAnim != null)
            return childAnim.transform;
        return null;
    }

    private Animator GetPipeAnimator()
    {
        return GetComponentInChildren<Animator>();
    }

    public void Rotate()
    {
        rotationState++;
        if (rotationState > 3)
            rotationState = 0;

        Transform pipeVisual = GetPipeVisual();
        Animator anim = GetPipeAnimator();

        if (anim != null)
            anim.enabled = false;

        if (pipeVisual != null)
            pipeVisual.rotation = Quaternion.Euler(0, 0, -90 * rotationState);

        RotateConnections();
        FindObjectOfType<PuzzleManager>().ActivateSystem();
    }

    public void ResetRotation()
    {
        rotationState = 0;

        Animator anim = GetPipeAnimator();
        if (anim != null)
            anim.enabled = false;

        Transform pipeVisual = GetPipeVisual();
        if (pipeVisual != null)
            pipeVisual.rotation = Quaternion.Euler(0, 0, 0);
    }

    void RotateConnections()
    {
        bool temp = up;
        // Clockwise rotation: up->right->down->left->up
        up = left;
        left = down;
        down = right;
        right = temp;
    }
}