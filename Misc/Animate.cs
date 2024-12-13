using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Animate : MonoBehaviour
{
    public Sprite[] images;
    public float animSpeed;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(AnimatePest());
    }


    public IEnumerator AnimatePest()
    {
        while (true)
        {
            int index = 0;
            while (index != images.Length)
            {
                gameObject.GetComponent<Image>().sprite = images[index];
                yield return new WaitForSeconds(animSpeed);
                index++;
            }
            index = 0;
        }
    }
}
