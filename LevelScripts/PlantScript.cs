using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class PlantScript : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public bool originalPlant = true;
    [HideInInspector] public bool isWatered;
    [HideInInspector] public bool isFertilized;
    [HideInInspector] public bool currentlyWP;
    [HideInInspector] public bool currentlyFP;
    [HideInInspector] public float growthTimer;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public int currentGrowthPhase = 0;
    [HideInInspector] public bool stopGrowing = false;

    [Header("Growth Setting")]
    public float firstGrowthTime = 10.0f;
    public float secondGrowthTime = 10.0f;
    public float witherTime = 30.0f;

    [Header("Attributes")]
    public int health = 30;
    public int sellPrice = 200;
    public int scoreValue = 20;
    public PlantType plant;
    //public Sprite[] plantSprites;
    public PlantPhases[] phases ;
    [Header("Additional Sprites")]
    //public Sprite firstWitherSprite;
    //public Sprite secondWitherSprite;
    public Sprite firstBrokenSprite;
    public Sprite secondBrokenSprite;

    [Header("Central Logic")]
    public GameObject levelProperties;

    [Header("Indicators")]
    public GameObject waterIndicator;
    public GameObject fertilizeIndicator;
    public GameObject harvestIndicator;
    public GameObject attackedIndicator;
    public GameObject deathIndicator;

    [Header("Audio")]
    public GameObject growAudio;
    public GameObject sellAudio;
    public GameObject witherAudio;

    [Header("Animation")]
    public float animSpeed = 0.03f;
    public float interlude = 0.005f;
    

    //private string[] growthPhases = new string[] { "sprout", "growing", "ripe", "withered"};
    private float[] growthTimerSet  ;
    private bool isAttacked = false;

    
    private RectTransform rectTransform;
    private GameObject indicatorPointer;
    
    // Start is called before the first frame update
    void Start()
    {
        growthTimerSet = new float[] { firstGrowthTime, secondGrowthTime, witherTime };
        growthTimer = growthTimerSet[currentGrowthPhase];

        isWatered = false;
        isFertilized = false;
        currentlyWP = false;
        currentlyFP = false;
        

        if (!originalPlant)
        {
            if (!isWatered || !isFertilized)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }

            if (!isWatered && !currentlyWP )
            {
                WaterPhase();
            }
            if (!isFertilized && !currentlyFP)
            {
                FertilizerPhase();
            }
            StartCoroutine(Animate());
            //Break
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            // Set the anchors to the center of the parent
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            // Optional: Reset position and size to align with the new anchors
            rectTransform.anchoredPosition = Vector2.zero; // Position at the center
            rectTransform.sizeDelta = Vector2.zero;        // No size offset
            //Break
            gameObject.GetComponent<RectTransform>().localPosition += new Vector3(0, 25); //note to future programmers, baris ini dibuat karena ada request buat plant yang lebih besar dengan margin yang kurang pas. Jadi plant digeser keatas buat solusi sementara
        }
    }

    private void Update()
    {
        
       // Debug.Log(growthTimer);

        if (!originalPlant && currentGrowthPhase != 3) {

            if (!isWatered || !isFertilized)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1);
            } else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
            //Debug.Log(!isWatered && !currentlyWP);
            if (!isWatered && !currentlyWP && currentGrowthPhase != 2)
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

            }
            if (isWatered && isFertilized && !stopGrowing)
            {
                growthTimer -= Time.deltaTime;
            }
        }
        if (health <= 0 && isAlive && currentGrowthPhase != 3)
        {
            gameObject.GetComponentInParent<SpriteRenderer>().sprite = gameObject.GetComponentInParent<PlotScript>().DryLand;
            BrokenPhase();
            //LevelProperties.Instance.GetComponent<StatsScript>().numOfPlants--;
            isAlive = false;
        }
    }

    public void GrowNext()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if (NotLastPhase()) //Pengondisian untuk mengecek tanaman belum sampai matang/tahap terakhir
        {
            StopCoroutine(Animate());

            //SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
            //spriteRender.sprite = plantSprites[currentGrowthPhase + 1];
            currentGrowthPhase++;
            
            growAudio.GetComponent<AudioSource>().Play();
            if (NotLastPhase()) {
                growthTimer = growthTimerSet[currentGrowthPhase];
            }

            isWatered = false;
            if (currentGrowthPhase == 2)
            {
                HarvestPhase();
            }
            else if (currentGrowthPhase == 3) {
                Destroy(indicatorPointer);
                Instantiate(deathIndicator, gameObject.transform);
                isAlive = false;
            }
            StartCoroutine(Animate());
        } else
        {
            
        }

    }

    public void WaterPhase()
    {
        //SpriteRenderer spriteInfo = GetComponent<SpriteRenderer>();
        //spriteInfo.color = new Color(0.4F,0.4F,0.4F,1);
        GameObject indicator = Instantiate(waterIndicator, transform);
        indicator.transform.SetParent(gameObject.transform);
        currentlyWP = true;
        isWatered = false;
    }

    public void FertilizerPhase()
    {
        //SpriteRenderer spriteInfo = GetComponent<SpriteRenderer>();

        
        GameObject indicator = Instantiate(fertilizeIndicator, transform);
        indicator.transform.SetParent(gameObject.transform);
        currentlyFP = true;
        isFertilized = false;
    }

    public void BrokenPhase()
    {
        if (currentGrowthPhase == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = firstBrokenSprite;
            
        } else if (currentGrowthPhase == 2)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = secondBrokenSprite;
        }
        currentGrowthPhase = -1;
        stopGrowing = true;
        StopAllCoroutines();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(indicatorPointer);
        Instantiate(deathIndicator, gameObject.transform);
    }
    public void OnPointerClick(PointerEventData eventData)
    {

        if (currentGrowthPhase == 2) {
            
            levelProperties.GetComponent<StatsScript>().moneyAvailable = levelProperties.GetComponent<StatsScript>().AddAmount(sellPrice, levelProperties.GetComponent<StatsScript>().moneyAvailable);
            levelProperties.GetComponent<StatsScript>().score = levelProperties.GetComponent<StatsScript>().AddAmount(scoreValue * 2, levelProperties.GetComponent<StatsScript>().score);
            levelProperties.GetComponent<StatsScript>().plantsHarvested = levelProperties.GetComponent<StatsScript>().AddAmount(1, levelProperties.GetComponent<StatsScript>().plantsHarvested);
            sellAudio.GetComponent<AudioSource>().Play();
            PlantAdd(plant);
            isWatered = false;
            if (!LevelProperties.Instance.unlimitedMoney)
            {
                GameObject floatingnum = LevelProperties.Instance.SpawnFloatingNumber(transform.parent, sellPrice);
                floatingnum.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            
            Destroy(gameObject);
            Destroy(indicatorPointer);
            

        }
    }

    public void BlinkEffect()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.3F, 0.3F, 0.3f, 0.8f);
    }

    public IEnumerator TakeDamage(int amount)
    {
        // Reduce health immediately
        health -= amount;

        // Check if health is below zero and let the external status updater handle it
        if (health <= 0)
        {
            yield break; // Exit the coroutine if the plant is "dead"
        }

        // Handle the attacked indicator
        if (!isAttacked)
        {
            isAttacked = true;

            // Show the indicator
            GameObject indicator = Instantiate(attackedIndicator, transform);
            indicator.transform.SetParent(gameObject.transform);

            // Wait for the indicator to disappear
            yield return new WaitForSeconds(1.5f);

            Destroy(indicator);
            isAttacked = false;
        }
        else
        {
            // Wait if already being attacked
            yield return new WaitForSeconds(1.5f);
        }
    }


    public bool NotLastPhase()
    {
        return currentGrowthPhase < phases.Length - 1;
    }

    public void HarvestPhase() 
    {
        
        isWatered = true;
        indicatorPointer = Instantiate(harvestIndicator);
        indicatorPointer.transform.SetParent(gameObject.transform.parent);
        indicatorPointer.transform.localPosition = new Vector3(0, 12, 35);
        indicatorPointer.transform.localScale = new Vector3(1.12f, 1.12f, 1.12f);
        
        
    }

    public void PlantAdd(PlantType plant)
    {
        switch (plant)
        {
            case PlantType.carrot:
                LevelProperties.Instance.GetComponent<StatsScript>().carrotsHarvested++;
                return ;
            case PlantType.potato:
                LevelProperties.Instance.GetComponent<StatsScript>().potatoHarvested++;
                return;
            case PlantType.yam:
                LevelProperties.Instance.GetComponent<StatsScript>().yamHarvested++;
                return;
        }
    }

    public IEnumerator Animate()
    {
        int index = 0;
        while (true)
        {
            while (index < phases[currentGrowthPhase].plantSprites.Length)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = phases[currentGrowthPhase].plantSprites[index];

                yield return new WaitForSeconds(animSpeed);
                index++;
            }
            index = 0;
        }

    }
}

public enum PlantType
{
    carrot,
    potato,
    yam,
    none

    
}

[System.Serializable]
public class PlantPhases
{
    public Sprite[] plantSprites;
}