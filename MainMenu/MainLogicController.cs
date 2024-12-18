using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class MainLogicController : MonoBehaviour
{
    [HideInInspector] public bool havePlayedBefore = false;
    public GameObject loadingScreen;
    public Slider loader;
    public GameObject warningObject;
    public static MainLogicController Instance { get; private set; }

    public void LoadScene(int scene)
    {
        //if (LevelProperties.Instance != null )
        //{
        //    level = LevelProperties.Instance.nextLevelIndex;
        //}
        ActivateLoadingScreen(scene);
        
        
        Time.timeScale = 1;
        //if (!havePlayedBefore)
        //{
        //    havePlayedBefore = true;
        //    PlayerPrefs.SetInt("havePlayed", 1);
        //}
    }

    public void PlayLevel(int level)
    {
        if ((PlayerPrefs.GetInt(SceneManager.GetSceneByBuildIndex(level).name + "-complete") == 0) && SaveFileExists(SceneManager.GetSceneByBuildIndex(level).name + "-saveFile.json"))
        {
            warningObject.SetActive(true);
            warningObject.GetComponent<WarningScreen>().desiredLevel = level;
        }else
        {
            LoadScene(level);
        }
    }

    public void ActivateLoadingScreen(int level)
    {
        loadingScreen.SetActive(true);
        
        StartCoroutine(Loading(level));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator Loading(int level)
    {
        yield return new WaitForSeconds(3);
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);
        
        while (!operation.isDone) {
            float progress = operation.progress;
            Debug.Log(progress);
            if (loader !=null)
            {
                loader.value = Mathf.Clamp01(progress / 0.9f);
                loader.SetValueWithoutNotify(Mathf.Clamp01(progress / 0.9f));
            }
            
            yield return null;
        }
    }

    public bool SaveFileExists(string fileName)
    {
        string filePath = Application.persistentDataPath + "/saves/" + fileName;
        return System.IO.File.Exists(filePath);
    }

}
