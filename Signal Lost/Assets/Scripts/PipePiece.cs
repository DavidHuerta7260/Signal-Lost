using UnityEngine;
using System.Collections;

public class PipePiece : MonoBehaviour
{
    private Animator animator;

    public void RefreshAnimator()
    {
        animator = GetComponentInChildren<Animator>();
        Debug.Log("RefreshAnimator on " + gameObject.name + " found: " + (animator != null ? animator.gameObject.name : "NULL"));
    }

    public void SetActive(bool active)
    {
        if (animator != null)
        {
            Quaternion savedRotation = animator.transform.rotation;

            animator.enabled = true;
            animator.SetBool("isActive", active);

            animator.transform.rotation = savedRotation;

            StopAllCoroutines();
            StartCoroutine(ReapplyRotation(savedRotation));
        }
        else
        {
            Debug.Log("PipePiece on " + gameObject.name + " has no animator - call RefreshAnimator first");
        }
    }

    IEnumerator ReapplyRotation(Quaternion rot)
    {
        yield return null;
        if (animator != null)
            animator.transform.rotation = rot;
    }
}