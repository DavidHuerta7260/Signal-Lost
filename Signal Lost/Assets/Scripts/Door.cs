using UnityEngine;

public class Door : MonoBehaviour
{
    void Start()
    {
        if (GameManager.puzzleSolved)
        {
            gameObject.SetActive(false);
        }
    }
}