using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlantScript : MonoBehaviour, IPointerClickHandler
{

    [HideInInspector] public bool originalPlant = true;

    [HideInInspector] public bool isWatered;
    [HideInInspector] public bool isFertilized;
    [HideInInspector] public bool currentlyWP;
    [HideInInspector] public bool currentlyFP;

    public float growthTime = 10.0f;
    public float timeUntilWater = 5.0f;
    public float timeUntilFertilized = 10.0f;
    public int health = 1000;
    public int sellPrice = 200;
    public int scoreValue = 20;

    public Sprite[] plantSprite;
    public GameObject eventSystem;
    public GameObject waterIndicator;
    public GameObject fertilizeIndicator;


    private int currentGrowthPhase = 0;
    [HideInInspector] public float growthTimer, waterTimer;

    // Start is called before the first frame update
    void Start()
    {
        growthTimer = growthTime;
        waterTimer = timeUntilWater;
        isWatered = false;
        isFertilized = false;
        //fertilizeTimer = timeUntilFertilized;
        if (!originalPlant)
        {
            if (waterTimer <= 0 && !currentlyWP && NotLastPhase() || !isWatered && !currentlyWP && NotLastPhase())
            {
                WaterPhase();
            }
            if (!isFertilized && !currentlyFP)
            {
                FertilizerPhase();
            }
            if (growthTimer <= 0 && isWatered && isFertilized)
            {
                GrowNext();
                growthTimer = growthTime;
            }
        }
    }

    private void Update()
    {
        growthTimer -= Time.deltaTime;
        waterTimer -= Time.deltaTime;
        //fertilizeTimer -= Time.deltaTime;

        if (!originalPlant && NotLastPhase()) {

            if (waterTimer <= 0 && !currentlyWP || !isWatered && !currentlyWP)
            {
                WaterPhase();
            }
            if (!isFertilized && !currentlyFP)
            {
                FertilizerPhase();
            }
            if (growthTimer <= 0 && isWatered && isFertilized)
            {
                GrowNext();
                growthTimer = growthTime;
            }
        }
    }

    public void GrowNext()
    {

        if (NotLastPhase()) //Pengondisian untuk mengecek tanaman belum sampai matang/tahap terakhir
        {
            SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
            spriteRender.sprite = plantSprite[currentGrowthPhase + 1];
            currentGrowthPhase++;

        }

    }

    public void WaterPhase()
    {
        SpriteRenderer spriteInfo = GetComponent<SpriteRenderer>();
        //spriteInfo.color = new Color(0.4F,0.4F,0.4F,1);

        Debug.Log("I Need Water!");
        GameObject indicator = Instantiate(waterIndicator, transform);
        indicator.transform.SetParent(gameObject.transform);
        currentlyWP = true;
        isWatered = false;
    }

    public void FertilizerPhase()
    {
        SpriteRenderer spriteInfo = GetComponent<SpriteRenderer>();

        Debug.Log("I Need Fertilizer!");
        GameObject indicator = Instantiate(fertilizeIndicator, transform);
        indicator.transform.SetParent(gameObject.transform);
        currentlyFP = true;
        isFertilized = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (HarvestPhase()) {
            Debug.Log("Harvested");
            eventSystem.GetComponent<StatsScript>().moneyAvailable = eventSystem.GetComponent<StatsScript>().addAmount(sellPrice, eventSystem.GetComponent<StatsScript>().moneyAvailable);
            eventSystem.GetComponent<StatsScript>().score = eventSystem.GetComponent<StatsScript>().addAmount(scoreValue * 2, eventSystem.GetComponent<StatsScript>().score);
            eventSystem.GetComponent<StatsScript>().plantsHarvested = eventSystem.GetComponent<StatsScript>().addAmount(1, eventSystem.GetComponent<StatsScript>().plantsHarvested);
            Destroy(gameObject);

        }
    }

    public void BlinkEffect()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.3F, 0.3F, 0.3f, 0.8f);
    }
    public void TakeDamage(int amount)
    {
        health -= amount;
    }

    public bool NotLastPhase()
    {
        return currentGrowthPhase < plantSprite.Length - 1;
    }

    public bool HarvestPhase() 
    {
        return currentGrowthPhase == plantSprite.Length - 1;
    }
}
