using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class PlantScript : MonoBehaviour, IPointerClickHandler
{
    public bool originalPlant = true;
    [HideInInspector] public bool currentlyWP = false;
    [HideInInspector] public bool currentlyFP = false;
    [HideInInspector] public bool currentlyHP = false;
    public bool currentlyWithered = false;
    public bool currentlyBroken = false;
    [HideInInspector] public float growthTimer;
    
    public int currentGrowthPhase = 0;
    [HideInInspector] public bool stopGrowing = false;
    public bool isWatered = false;
    public bool isFertilized = false;
    public bool isAlive = true;
    public bool isReadyToHarvest = false;
    

    [Header("Growth Setting")]
    public float sproutGrowthTime = 10.0f;
    public float plantGrowthTime = 10.0f;
    public bool canWither = true;
    public float witherTime,witherTimer = 20.0f;


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
    public Sprite firstWitherSprite;
    public Sprite secondWitherSprite;

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

    

    //private string[] growthPhases = new string[] { "sprout", "growing", "ripe", "withered"};
    private float[] growthTimerSet  ;
    private bool isAttacked = false;
    
    private RectTransform rectTransform;
    [HideInInspector] public GameObject indicatorPointer;
    
    // Start is called before the first frame update
    void Start()
    {
        growthTimerSet = new float[] { sproutGrowthTime, plantGrowthTime };
        if (currentGrowthPhase >= 2)
        {
            growthTimer = growthTimerSet[1];
        } else
        {
            growthTimer = growthTimerSet[0];
        }
        ;

        if (!originalPlant)
        {
            rectTransform = GetComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            if (currentlyWithered)
            {
                if (plant == 0)
                {
                    gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                }
            }
            else
            {
                gameObject.GetComponent<RectTransform>().localPosition += new Vector3(0, 25);
            }
            //note to future programmers, baris diatas dibuat karena ada request buat plant yang lebih besar dengan margin yang kurang pas. Jadi plant digeser keatas buat solusi sementara
 
                if ((!isWatered || !isFertilized))
                {
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1);

                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                }

                if (!isWatered && !currentlyWP)
                {

                    WaterPhase();
                }
                if (!isFertilized && !currentlyFP)
                {
                    FertilizerPhase();
                }
                if (isReadyToHarvest && !currentlyHP)
                {
                    HarvestPhase();
                }
                StartCoroutine(AnimateSprite());
            

        }
    }

    private void Update()
    {
        
       // Debug.Log(growthTimer);

        if (!originalPlant /*&& currentGrowthPhase != phases.Length-1*/&& LevelProperties.Instance.gameOngoing) {

            if ((!isWatered || !isFertilized) && isAlive)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }

            if ((!isWatered || isReadyToHarvest) && currentGrowthPhase != 0 && witherTimer >= 0 && canWither && LevelProperties.Instance.gameOngoing)
            {
                Debug.Log("Withering :" + witherTimer);
                witherTimer -= Time.deltaTime;
            }

            if (isAlive && !isWatered && !currentlyWP  && currentGrowthPhase != phases.Length-1)
            {
                WaterPhase();
            }

            if (!isFertilized && !currentlyFP && isAlive)
            {
                FertilizerPhase();
            }
            if (isReadyToHarvest && !currentlyHP)
            {
                HarvestPhase();
            }
            if (growthTimer <= 0 && isWatered && isFertilized && isAlive)
            {
                GrowNext();
            }

            if (isWatered && isFertilized && !stopGrowing && currentGrowthPhase != phases.Length - 1)
            {
                growthTimer -= Time.deltaTime;
            }

            if (witherTimer <= 0 && isAlive)
            {
                Debug.Log("Withered");
                WitherPhase();
            }
        }
        if (health <= 0 && isAlive)
        {
            gameObject.GetComponentInParent<SpriteRenderer>().sprite = gameObject.GetComponentInParent<PlotScript>().DryLand;
            BrokenPhase();
            //LevelProperties.Instance.GetComponent<StatsScript>().numOfPlants--;
            
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
            StopAllCoroutines();

            //SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
            //spriteRender.sprite = plantSprites[currentGrowthPhase + 1];
            currentGrowthPhase++;
            
            growAudio.GetComponent<AudioSource>().Play();
            if (NotLastPhase()) {
                if (currentGrowthPhase >= 1)
                {
                    growthTimer = growthTimerSet[1];
                } else
                {
                    growthTimer = growthTimerSet[currentGrowthPhase];
                }
            }

            isWatered = false;
            if (currentGrowthPhase == phases.Length-1)
            {
                isReadyToHarvest = true;
            }
            //else if (currentGrowthPhase == 3) {
            //    Destroy(indicatorPointer);
            //    Instantiate(deathIndicator, gameObject.transform);
            //    isAlive = false;
            //}
            StartCoroutine(AnimateSprite());
        } else
        {
            stopGrowing = true;
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
        currentlyBroken = true;
        if (currentGrowthPhase == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = firstBrokenSprite;
            currentGrowthPhase = -1;

        } else if (currentGrowthPhase >= 2)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = secondBrokenSprite;
            currentGrowthPhase = -2;
        }
        gameObject.transform.localScale = Vector3.one;
        //gameObject.transform.localPosition = Vector3.zero;
        gameObject.GetComponent<GridLayoutGroup>().padding.top = 0;
        gameObject.transform.parent.GetComponent<PlotScript>().toolActive = false;

        isWatered = true;
        isFertilized = true;
        currentlyWP = false;
        currentlyFP = false;

        isReadyToHarvest = false;
        stopGrowing = true;
        isAlive = false;
        levelProperties.GetComponent<StatsScript>().numOfPlants--;
        levelProperties.GetComponent<StatsScript>().plantsLost++;
        StopAllCoroutines();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if (indicatorPointer != null)
        {
            Destroy(indicatorPointer);
        }
        Instantiate(deathIndicator, gameObject.transform);
    }

    public void WitherPhase()
    {
        currentlyWithered = true;
        if (currentGrowthPhase == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = firstWitherSprite;
            currentGrowthPhase = -3;

        }
        else if (currentGrowthPhase >= 2)
        {
            Debug.Log("withered2");
            gameObject.GetComponent<SpriteRenderer>().sprite = secondWitherSprite;
            currentGrowthPhase = -4;
        }
        gameObject.transform.localScale = Vector3.one;
        if (plant == PlantType.carrot)
        {
            gameObject.transform.localPosition = Vector3.zero;
        } // if statement ini khusus ke wortel karena sprite lain fine fine aja, tapi wortel malah keatas sendiri. not good case practice ik but i'll work with the sprites i got
        gameObject.transform.parent.GetComponent<PlotScript>().toolActive = false;

        isWatered = true;
        isFertilized = true;
        currentlyWP = false;
        currentlyFP = false;

        gameObject.GetComponent<GridLayoutGroup>().padding.top = 0;
        isReadyToHarvest = false;

        stopGrowing = true;
        isAlive = false;
        levelProperties.GetComponent<StatsScript>().numOfPlants--;
        levelProperties.GetComponent<StatsScript>().plantsLost++;
        StopAllCoroutines();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if (indicatorPointer != null)
        {
            Destroy(indicatorPointer);
        }
        Instantiate(deathIndicator, gameObject.transform);
    }
    public void OnPointerClick(PointerEventData eventData)
    {

        if (isReadyToHarvest) {
            
            levelProperties.GetComponent<StatsScript>().moneyAvailable = levelProperties.GetComponent<StatsScript>().AddAmount(sellPrice, levelProperties.GetComponent<StatsScript>().moneyAvailable);
            levelProperties.GetComponent<StatsScript>().score = levelProperties.GetComponent<StatsScript>().AddAmount(scoreValue * 2, levelProperties.GetComponent<StatsScript>().score);
            levelProperties.GetComponent<StatsScript>().plantsHarvested = levelProperties.GetComponent<StatsScript>().AddAmount(1, levelProperties.GetComponent<StatsScript>().plantsHarvested);
            sellAudio.GetComponent<AudioSource>().Play();
            PlantAdd(plant);
            
            
            isWatered = false;
            //gameObject.GetComponentInParent<SpriteRenderer>().sprite = gameObject.GetComponentInParent<PlotScript>().DryLand;
            if (!LevelProperties.Instance.unlimitedMoney)
            {
                GameObject floatingnum = LevelProperties.Instance.SpawnFloatingNumber(transform.parent, sellPrice);
                floatingnum.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            
            
            gameObject.GetComponentInParent<PlotScript>().hasPlant = false;
            Destroy(indicatorPointer);
            Destroy(gameObject);

        }
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
    }


    public bool NotLastPhase()
    {
        return currentGrowthPhase < phases.Length - 1;
    }

    public void HarvestPhase() 
    {
        currentlyHP = true;
        isWatered = true;
        gameObject.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        indicatorPointer = Instantiate(harvestIndicator);
        indicatorPointer.transform.SetParent(gameObject.transform.parent);
        indicatorPointer.transform.localPosition = new Vector3(0, 12, 35);
        indicatorPointer.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        stopGrowing = true;
        isReadyToHarvest = true;
        
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

    public IEnumerator AnimateSprite()
    {
        int index = 0;
        
        if (currentGrowthPhase >= 0)
        {
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
}


[System.Serializable]
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