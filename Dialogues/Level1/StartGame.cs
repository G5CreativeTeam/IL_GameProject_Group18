using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] essentialObjects;
    public GameObject[] unEssentialObjects;

    void Start()
    {
        foreach (GameObject obj in essentialObjects) { 
            obj.SetActive(true);
            if (obj.GetComponent<LevelProperties>() != null)
            {
                obj.GetComponent<LevelProperties>().InitiateGame();
                obj.GetComponent<LevelProperties>().activateDialogue = true;
            }
        }
        foreach (GameObject obj in unEssentialObjects)
        {
            obj.SetActive(false);
        }
    }

}
