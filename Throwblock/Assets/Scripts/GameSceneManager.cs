using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;
    public float uiLoadTime = 0.5f;
    private AsyncOperation asyncOperation;
    public Scene currentScene;

    private void Awake()
    {
        Instance = this;
        currentScene = SceneManager.GetActiveScene();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadNewScene(sceneName));
    }

    private IEnumerator LoadNewScene(string sceneName)
    {
        yield return null;
        Time.timeScale = 1f;

        yield return new WaitForSecondsRealtime(uiLoadTime);
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}