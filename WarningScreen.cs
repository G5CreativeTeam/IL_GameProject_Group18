using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WarningScreen : MonoBehaviour
{
    [HideInInspector] public int desiredLevel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void PlayNewGame()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "-Continue", 0);
        MainLogicController.Instance.LoadScene(desiredLevel);
    }

    void ContinueGame()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "-Continue", 1);
        MainLogicController.Instance.LoadScene(desiredLevel);
    }
}
