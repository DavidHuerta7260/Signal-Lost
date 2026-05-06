using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    public string[] dialogueLines;
    public string puzzleSceneName = "PuzzleScene";

    private int currentLine = 0;
    private bool isTalking = false;

    void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }

    public void Interact()
    {
        isTalking = true;
        currentLine = 0;
        ShowLine();
    }

    void ShowLine()
    {
        Debug.Log(dialogueLines[currentLine]);
    }

    void NextLine()
    {
        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            ShowLine();
        }
        else
        {
            isTalking = false;
            LoadPuzzle();
        }
    }

    void LoadPuzzle()
    {
        GameManager.playerPosition = GameObject.FindWithTag("Player").transform.position;
        GameManager.hasSavedPosition = true;

        SceneManager.LoadScene(puzzleSceneName);
    }
}
