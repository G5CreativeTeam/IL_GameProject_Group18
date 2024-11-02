using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    public int growthTime = 10;
    public int timeUntilWater = 45;
    public int timeWithoutWater = 30;
    public int health = 1000;
    public Sprite[] plantSprite;
    public bool originalPlant = true;

    private int currentGrowthPhase = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!originalPlant) {
            InvokeRepeating("GrowNext", 3.0f, growthTime);

        }
    }

    public void GrowNext()
    {
        SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
        spriteRender.sprite = plantSprite[currentGrowthPhase+1];
    }
}
