using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public Animator anim;
    public bool isTalking;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isTalking = false;
    }

    


}