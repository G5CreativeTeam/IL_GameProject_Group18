using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedMask : MonoBehaviour
{
    public GameObject Seed;

    private int emptyCoordinates = 45;
    private float percentage;


    void Start()
    {
        percentage = Seed.GetComponent<SeedScript>().cooldownTimer / Seed.GetComponent<SeedScript>().cooldownTime;
        Debug.Log(Seed.GetComponent<SeedScript>().cooldownTimer + " / " + Seed.GetComponent<SeedScript>().cooldownTime);
        gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0,-percentage);
    }

    // Update is called once per frame
    void Update()
    {
        percentage = Seed.GetComponent<SeedScript>().cooldownTimer / Seed.GetComponent<SeedScript>().cooldownTime;
        Debug.Log(percentage);
        gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, -percentage*emptyCoordinates);
    }
}
