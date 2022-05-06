using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeToScene : MonoBehaviour
{
    public Button startButton;
    private string sceneMainMenu = "MainMenu";
    private string sceneLevelOne = "LevelOne";
    private string sceneLevelTwo = "LevelTwo";

    private void Start()
    {
        startButton.onClick.AddListener(LoadSceneLevelOne);
    }

    public void LoadSceneMainMenu()
    {
        GameSceneManager.Instance.LoadScene(sceneMainMenu);
    }

    public void LoadSceneLevelOne()
    {
        GameSceneManager.Instance.LoadScene(sceneLevelOne);
    }

    public void LoadSceneLevelTwo()
    {
        GameSceneManager.Instance.LoadScene(sceneLevelTwo);
    }
}
