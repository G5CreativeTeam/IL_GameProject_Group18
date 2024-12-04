using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    // Start is called before the first frame update
    public int level;
    void Start()
    {
        SceneManager.LoadSceneAsync(level);   
    }
}
