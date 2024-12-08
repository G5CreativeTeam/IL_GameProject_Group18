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
    [HideInInspector] public bool originalPlant = true;
    [HideInInspector] public bool currentlyWP;
    [HideInInspector] public bool currentlyFP;
    [HideInInspector] public float growthTimer;
    [HideInInspector] public float witherTimer;
    [HideInInspector] public int currentGrowthPhase = 0;
    [HideInInspector] public bool stopGrowing = false;
    [HideInInspector] public bool isWatered;
    [HideInInspector] public bool isFertilized;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool isReadyToHarvest = false;

    [Header("Growth Setting")]
    public float sproutGrowthTime = 10.0f;
    public float plantGrowthTime = 10.0f;
    public float witherTime = 20.0f;

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
    private GameObject indicatorPointer;
    
    // Start is called before the first frame update
    void Start()
    {
        growthTimerSet = new float[] { sproutGrowthTime, plantGrowthTime };
        growthTimer = growthTimerSet[currentGrowthPhase];
        witherTimer = witherTime;
        isWatered = false;
        isFertilized = false;
        currentlyWP = false;
        currentlyFP = false;
        

        if (!originalPlant)
        {
            if ((!isWatered || !isFertilized) )
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1);

            } else
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
            StartCoroutine(Animate());



            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            rectTransform.anchoredPosition = Vector2.zero; 
            rectTransform.sizeDelta = Vector2.zero;        
            
            gameObject.GetComponent<RectTransform>().localPosition += new Vector3(0, 25); 
            //note to future programmers, baris diatas dibuat karena ada request buat plant yang lebih besar dengan margin yang kurang pas. Jadi plant digeser keatas buat solusi sementara
        }
    }

    private void Update()
    {
        
       // Debug.Log(growthTimer);

        if (!originalPlant /*&& currentGrowthPhase != phases.Length-1*/) {

            if ((!isWatered || !isFertilized))
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }

            if ((!isWatered || isReadyToHarvest) && currentGrowthPhase != 0 && witherTimer >= 0)
            {
                Debug.Log("Withering :" + witherTimer);
                witherTimer -= Time.deltaTime;
            }
            //Debug.Log(!isWatered && !currentlyWP);
            if (!isWatered && !currentlyWP && currentGrowthPhase != phases.Length-1)
            {
                WaterPhase();
            }
            if (!isFertilized && !currentlyFP)
            {
                FertilizerPhase();
            }
            if (growthTimer <= 0 && isWatered && isFertilized && isAlive)
            {
                GrowNext();

            }
            if (isWatered && isFertilized && !stopGrowing && currentGrowthPhase != phases.Length - 1)
            {
                growthTimer -= Time.deltaTime;
            }
            if (witherTimer <= 0 && isAlive )
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
            //StopCoroutine(Animate());

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
                gameObject.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
                HarvestPhase();
            }
            //else if (currentGrowthPhase == 3) {
            //    Destroy(indicatorPointer);
            //    Instantiate(deathIndicator, gameObject.transform);
            //    isAlive = false;
            //}
            StartCoroutine(Animate());
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
        if (currentGrowthPhase == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = firstBrokenSprite;
            
        } else if (currentGrowthPhase >= 2)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = secondBrokenSprite;
        }
        isReadyToHarvest = false;
        currentGrowthPhase = -1;
        stopGrowing = true;
        isAlive = false;
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
        if (currentGrowthPhase == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = firstWitherSprite;

        }
        else if (currentGrowthPhase >= 2)
        {
            Debug.Log("withered2");
            gameObject.GetComponent<SpriteRenderer>().sprite = secondWitherSprite;
        }
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.GetComponent<GridLayoutGroup>().padding.top = 0;
        isReadyToHarvest = false;
        currentGrowthPhase = -1;
        stopGrowing = true;
        isAlive = false;
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
            
            Destroy(gameObject);
            gameObject.GetComponentInParent<PlotScript>().hasPlant = false;
            Destroy(indicatorPointer);
            

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
        
        isWatered = true;
        indicatorPointer = Instantiate(harvestIndicator);
        indicatorPointer.transform.SetParent(gameObject.transform.parent);
        indicatorPointer.transform.localPosition = new Vector3(0, 12, 35);
        indicatorPointer.transform.localScale = new Vector3(1.12f, 1.12f, 1.12f);
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

    public IEnumerator Animate()
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