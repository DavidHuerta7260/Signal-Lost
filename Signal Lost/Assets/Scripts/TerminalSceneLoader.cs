using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerminalSceneLoader : MonoBehaviour
{
    public void LoadAfterDelay(string sceneName, float delay)
    {
        StartCoroutine(LoadRoutine(sceneName, delay));
    }

    IEnumerator LoadRoutine(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}