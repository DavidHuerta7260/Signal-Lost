using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "MainLevel";

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
