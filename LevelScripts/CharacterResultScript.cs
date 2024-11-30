using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterResultScript : MonoBehaviour
{
    public GameObject WinCharacter;
    public GameObject LoseCharacter;
    public GameObject eventSystem;

    private bool status;
    // Start is called before the first frame update
    void OnEnable()
    {
        foreach (GameObjective target in LevelProperties.Instance.Targets)
        {
            status = target.completed;
            if (!status)
            {
                break;
            }
        }
        if (status)
        {
            WinCharacter.SetActive(true);
        } else
        {
            LoseCharacter.SetActive(true);
        }

    }
}
