using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleComplete : MonoBehaviour
{
    public string mainSceneName = "MainLevel";
    public GameObject completePanel;

    public void CompletePuzzle()
    {
        Debug.Log("Puzzle Complete!");

        GameManager.puzzleSolved = true;

        completePanel.SetActive(true);
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}
