using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainLogicController : MonoBehaviour
{
    
    [HideInInspector] public bool havePlayedBefore = false;

    public void PlayGame(int level)
    {
        SceneManager.LoadSceneAsync(level);
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
}
