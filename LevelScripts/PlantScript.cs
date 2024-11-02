using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    public float growthTime = 10.0f;
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
            InvokeRepeating("GrowNext", growthTime, growthTime);
            InvokeRepeating("Test", growthTime, growthTime);
        }
    }

    public void GrowNext()
    {
        Debug.Log(currentGrowthPhase <= plantSprite.Length);
        if (currentGrowthPhase < plantSprite.Length)
        {
            SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
            currentGrowthPhase++;
            spriteRender.sprite = plantSprite[currentGrowthPhase];
        }
        Debug.Log(currentGrowthPhase);
    }

    public void Test()
    {
        
    }
}
