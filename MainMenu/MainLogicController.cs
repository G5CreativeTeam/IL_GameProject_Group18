using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainLogicController : MonoBehaviour
{
    [HideInInspector] public bool havePlayedBefore = false;
    public GameObject loadingScreen;
    public Slider loader;
    
    public void PlayGame(int level)
    {
        if (LevelProperties.Instance.nextLevelIndex >= 0)
        {
            level = LevelProperties.Instance.nextLevelIndex;
        }
        loadingScreen.SetActive(true);
        
        StartCoroutine(Loading(level));
        Time.timeScale = 1;
        if (!havePlayedBefore)
        {
            havePlayedBefore = true;
            PlayerPrefs.SetInt("havePlayed", 1);
        }
        
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator Loading(int level)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);
        
        while (!operation.isDone) {
            float progress = operation.progress;
            Debug.Log(progress);
            loader.value = Mathf.Clamp01(progress / 0.9f);
            loader.SetValueWithoutNotify(Mathf.Clamp01(progress / 0.9f));
            yield return null;
        }
    }
}
