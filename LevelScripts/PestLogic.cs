using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PestLogic : MonoBehaviour, IDataPersistence
{
    [Header("Properties")]
    //public int maxHealth = 1;
    public float attackRate = 2;
    public int damage = 2;
    public float speed = 3;
    public int size = 1;
    public float swipeSpeed = 5f;
    public float minSwipeDistance = 0.5f; // Minimum distance to qualify as a swipe
    public PestType type;
    public PestManager manager;

    [Header("Animation")]
    public float animSpeed;
    public Sprite[] animationFrames;

    [Header("Audio")]
    public GameObject attackAudio;

    [Tooltip("Only if on the menu")]
    public GameObject menuPestSpawner;

    [ReadOnly] public GameObject target;
    [ReadOnly] public bool originalPest;
    [ReadOnly] public bool isCurrentlyColliding;

    [ReadOnly] public float attackTimer;
    [ReadOnly] private GameObject currentPlant; // Tracks the plant being attacked
    private List<GameObject> otherColliders = new(); // Tracks other colliding objects
    private Rigidbody2D rb;

    private Vector2 swipeStart;
    private Vector2 swipeEnd;
    private bool isSwiping = false;
    private bool hasSwiped = false;
    private bool newSpawn = true;
    private bool isStopped = false; // Tracks if the pest is stopped
    private bool isSpinning = false; // Tracks if the pest is spinning
    private float spinDirection = 1f; // 1 for clockwise, -1 for counterclockwise
    private float temp;
    private int index;
    private Vector3 previousPosition;
    private Vector2 roamDirection;
    private float roamDuration;
    private float roamTimer;
    [SerializeField] private double flipThreshold = 0.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000001d; // Minimum movement to trigger a flip


    public void Start()
    {


        temp = speed;
        rb = GetComponent<Rigidbody2D>();
        attackTimer = 0;
        FindNearestPlant();
        previousPosition = GetComponent<RectTransform>().position; 
        if (!originalPest)
        {
            StartCoroutine(Animate());
        }
    }



// Roaming randomly


// Update Method Modification
public void Update()
{
    if (isSpinning)
    {
            GetComponent<RectTransform>().Rotate(Vector3.forward * 360 * spinDirection * Time.deltaTime);
    }

    if (PestOutOfScreen() && !newSpawn)
    {
        PestDeath();
    }

    if (isStopped)
    {
        HandleSwipe(); // Allow swiping or resuming movement
        return; // Skip other behavior while stopped
    }

    target = FindNearestPlant();

    if (newSpawn && !PestOutOfScreen())
    {
        newSpawn = false;
    }

    if (target != null && !originalPest && !hasSwiped)
    {
        TowardsPlant(target);
    }
    else if (target == null && !originalPest)
    {
        RoamRandomly();
    }

    if (currentPlant != null)
    {
        PlantScript plant = currentPlant.GetComponent<PlantScript>();
        if (plant.isAlive && plant.currentGrowthPhase != 0)
        {
            attackTimer += Time.deltaTime;
            StopMoving();

            if (attackTimer >= attackRate)
            {
                Debug.Log("Attacking");
                attackAudio.GetComponent<AudioSource>().Play();
                StartCoroutine(plant.TakeDamage(damage));
                attackTimer = 0;
            }
            else
            {
                Debug.Log("Awaiting: " + (attackRate - attackTimer));
            }
        }
        else
        {
            ResumeMoving();
        }
    }
    else
    {
        ResumeMoving();
    }

    if (!newSpawn)
    {
        HandleSwipe();
    }

    FlipSprite();

        if (LevelProperties.Instance != null && !originalPest && target == null)
        {
            //KeepWithinScreenBounds();
        } else
        {
            if (!isStopped && !hasSwiped && !originalPest && PestOutOfScreen() )
            {
                MoveTowardsCameraCenter();
            }

        }
    }


    private void StopMoving()
    {
        speed = 0;
    }

    private void ResumeMoving()
    {
        speed = temp;
    }

    private void HandleSwipe()
    {
        if (isSwiping && Input.GetMouseButtonUp(0))
        {
            Debug.Log("End");

            swipeEnd = Input.mousePosition;
            float swipeDistance = Vector2.Distance(swipeStart, swipeEnd);

            if (swipeDistance >= minSwipeDistance)
            {
                Debug.Log("Swiped");
                Vector2 swipeDirection = (swipeEnd - swipeStart).normalized;
                spinDirection = swipeDirection.x > 0 ? -1f : 1f;

                rb.velocity = swipeDirection * swipeSpeed;
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
                hasSwiped = true;
                isSpinning = true;
            }
            else
            {
                isStopped = false;
                StartCoroutine(Animate());
            }

            isSwiping = false;
        }
    }

    public void OnMouseDown()
    {
        Debug.Log("Pointer Clicked");
        if (!isStopped && !hasSwiped)
        {
            Debug.Log("Pest clicked!");
            swipeStart = Input.mousePosition;
            isStopped = true;
            rb.velocity = Vector2.zero;
            StopCoroutine(Animate());
            isSwiping = true;
        }
    }


    public void OnCollisionEnter2D(Collision2D col)
    {
        if (!hasSwiped)
        {
            if (col.gameObject.TryGetComponent<PlantScript>(out _))
            {
                currentPlant = col.gameObject;
            }
            else
            {
                otherColliders.Add(col.gameObject);
            }
        }
    }

    public void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject == currentPlant)
        {
            currentPlant = null;
            attackTimer = 0;
        }
        else if (otherColliders.Contains(col.gameObject))
        {
            otherColliders.Remove(col.gameObject);
        }
    }

    public void OnMouseOver()
    {
        CursorManager.Instance.SetCursorHover();
    }

    public void OnMouseExit()
    {
        CursorManager.Instance.SetCursorDefault();
    }

    public void TowardsPlant(GameObject target)
    {
        if (target != null)
        {
            if (LevelProperties.Instance != null)
            {
                GetComponent<RectTransform>().position = Vector2.MoveTowards(
                GetComponent<RectTransform>().position,
                target.GetComponent<RectTransform>().position,
                LevelProperties.Instance.pestSpeedMultiplier * speed * Time.deltaTime
                );
            } else
            {
                GetComponent<RectTransform>().position = Vector2.MoveTowards(
                GetComponent<RectTransform>().position,
                target.transform.position,
                    1* speed * Time.deltaTime
                );
            }
            
        }
    }

    public GameObject FindNearestPlant()
    {
        PlantScript[] plants = FindObjectsOfType<PlantScript>();
        float nearestDistance = Mathf.Infinity;
        Camera mainCamera = Camera.main;
        GameObject targetPlant = null;

        foreach (PlantScript plant in plants)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(plant.transform.position);
            if (plant.isAlive && viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
            {
                float distance = Vector2.Distance(GetComponent<RectTransform>().position, plant.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    targetPlant = plant.gameObject;
                }
            }
        }

        return targetPlant;
    }

    public void PestDeath()
    {
        Destroy(gameObject);
        if (LevelProperties.Instance != null)
        {
            LevelProperties.Instance.GetComponent<StatsScript>().numOfPests--;
        } else
        {
            menuPestSpawner.GetComponent<MenuPestSpawner>().numOfPests--;
        }
        
    }

    public bool PestOutOfScreen()
    {
        Bounds bounds = GetComponent<Collider2D>().bounds;
        Camera mainCamera = Camera.main;

        Vector3 minViewport = mainCamera.WorldToViewportPoint(bounds.min);
        Vector3 maxViewport = mainCamera.WorldToViewportPoint(bounds.max);

        return maxViewport.x < 0 || minViewport.x > 1 || maxViewport.y < 0 || minViewport.y > 1;
    }



    private void FlipSprite()
    {
        // Calculate the horizontal movement
        double horizontalMovement = GetComponent<RectTransform>().position.x - previousPosition.x;
        // Check if the movement exceeds the threshold
        
        if (Math.Abs(horizontalMovement) > flipThreshold)
        {
            if (horizontalMovement < 0) // Moving left
            {
                GetComponent<RectTransform>().localScale = new Vector3(Mathf.Abs(GetComponent<RectTransform>().localScale.x), GetComponent<RectTransform>().localScale.y, GetComponent<RectTransform>().localScale.z);
            }
            else if (horizontalMovement > 0) // Moving right
            {
                GetComponent<RectTransform>().localScale = new Vector3(-Mathf.Abs(GetComponent<RectTransform>().localScale.x), GetComponent<RectTransform>().localScale.y, GetComponent<RectTransform>().localScale.z);
            }
        }

        // Update the previous position for the next frame
        previousPosition = GetComponent<RectTransform>().position;
    }

    private void MoveTowardsCameraCenter()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found.");
            return;
        }

        // Get the viewport position of the pest
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(GetComponent<RectTransform>().position);

        // Check if the pest is outside the camera area
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            // Calculate the direction towards the center of the camera's viewport
            Vector3 screenCenter = new(0.5f, 0.5f, viewportPos.z);
            Vector3 worldCenter = mainCamera.ViewportToWorldPoint(screenCenter);
            Vector3 directionToCenter = (worldCenter - GetComponent<RectTransform>().position).normalized;

            // Move the pest towards the center
            GetComponent<RectTransform>().Translate(directionToCenter * speed * Time.deltaTime, Space.World);
        }
    }

    private void RoamRandomly()
    {
        if (roamTimer <= 0)
        {
            // Generate a new random direction and duration
            roamDirection = UnityEngine.Random.insideUnitCircle.normalized; // Random normalized vector
            roamDuration = UnityEngine.Random.Range(1f, 3f); // Random duration between 1 to 3 seconds
            roamTimer = roamDuration;
        }

        // Move in the chosen direction
        GetComponent<RectTransform>().Translate(roamDirection * speed * Time.deltaTime);

        // Decrease the roam timer
        roamTimer -= Time.deltaTime;

        // Prevent moving out of bounds
        if (LevelProperties.Instance != null)
        {
            KeepWithinScreenBounds();
        }

    }

    private void KeepWithinScreenBounds()
    {
        Camera mainCamera = Camera.main;
        Vector3 position = GetComponent<RectTransform>().position;
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(position);

        // Clamp position to stay within the screen
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0.05f, 0.95f); // Add padding to avoid edge glitches
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0.05f, 0.95f);

        GetComponent<RectTransform>().position = mainCamera.ViewportToWorldPoint(viewportPos);
    }




    public IEnumerator Animate()
    {
        while (true)
        {
            while (index < animationFrames.Length)
            {
                GetComponent<SpriteRenderer>().sprite = animationFrames[index];
                yield return new WaitForSeconds(animSpeed);
                index++;
            }
            index = 0;
        }
    }

    public void LoadData(GameData gameData)
    {
        Debug.Log("Loaded ig?");
    }

    public void SaveData(ref GameData gameData)
    {
        PestData pestData = new();
        pestData.x = gameObject.transform.position.x;
        pestData.y = gameObject.transform.position.y;
        pestData.z = gameObject.transform.position.z;
        pestData.scaleX = gameObject.transform.localScale.x;
        pestData.scaleY = gameObject.transform.localScale.y;
        pestData.scaleZ = gameObject.transform.localScale.z;
        pestData.speed = speed;
        pestData.originalPest = originalPest;
        pestData.type = type;
        Debug.Log("Jalan ga sih");

        if (!originalPest)
        {
            
            Debug.Log("Masuk"+pestData);
            manager.data.Add(pestData);
            Debug.Log(gameData.pestList);
            Debug.Log("keluar");
        }
        



        //PlotData plotSave = new();
        
        //plotSave.id = id;
        //plotSave.hasPlant = hasPlant;
        //plotSave.PlantType = type;
        //PlantScript plant;
        //plotSave.plantData = new();
        //if (plotSave.hasPlant && plantObject != null)
        //{
        //    plant = plantObject.GetComponent<PlantScript>();

        //    plotSave.plantData.health = plant.health;
        //    plotSave.plantData.currentlyWP = plant.currentlyWP;
        //    plotSave.plantData.currentlyFP = plant.currentlyFP;
        //    Debug.Log(plant.currentlyWithered);
        //    plotSave.plantData.currentlyWithered = plant.currentlyWithered;
        //    plotSave.plantData.currentlyBroken = plant.currentlyBroken;
        //    plotSave.plantData.growthTimer = plant.growthTimer;
        //    plotSave.plantData.witherTimer = plant.witherTimer;
        //    plotSave.plantData.currentGrowthPhase = plant.currentGrowthPhase;
        //    plotSave.plantData.stopGrowing = plant.stopGrowing;


        //    plotSave.plantData.isWatered = plant.isWatered;


        //    plotSave.plantData.isFertilized = plant.isFertilized;
        //    plotSave.plantData.isAlive = plant.isAlive;
        //    plotSave.plantData.isReadyToHarvest = plant.isReadyToHarvest;
        //}


        //PlotData matchingPlot = gameData.plotList.Find(plot => plot.id == id);
        ////matchingPlot.plantData = new();

        //if (matchingPlot != null)
        //{
        //    matchingPlot.hasPlant = hasPlant;
        //    matchingPlot.PlantType = type;

        //    matchingPlot.plantData.isWatered = plotSave.plantData.isWatered;
        //    matchingPlot.plantData.isFertilized = plotSave.plantData.isFertilized;
        //    matchingPlot.plantData.health = plotSave.plantData.health;
        //    matchingPlot.plantData.currentlyWP = plotSave.plantData.currentlyWP;
        //    matchingPlot.plantData.currentlyFP = plotSave.plantData.currentlyFP;
        //    matchingPlot.plantData.currentlyWithered = plotSave.plantData.currentlyWithered;
        //    matchingPlot.plantData.currentlyBroken = plotSave.plantData.currentlyBroken;
        //    matchingPlot.plantData.growthTimer = plotSave.plantData.growthTimer;
        //    matchingPlot.plantData.witherTimer = plotSave.plantData.witherTimer;
        //    matchingPlot.plantData.currentGrowthPhase = plotSave.plantData.currentGrowthPhase;
        //    matchingPlot.plantData.stopGrowing = plotSave.plantData.stopGrowing;
        //    matchingPlot.plantData.isAlive = plotSave.plantData.isAlive;
        //    matchingPlot.plantData.isReadyToHarvest = plotSave.plantData.isReadyToHarvest;

        //}
        //else
        //{
        //    //List<plotData> list = new();
        //    //list.Add(plotSave);
        //    gameData.plotList.Add(plotSave);
        //    //foreach (PlotData plotData in gameData.plotList)
        //    //{
        //    //    Debug.Log(plotData.hasPlant);
        //    //}
        //}


    }



    //public IEnumerator Roam()
    //{
    //    int randomDirection = Random.Range(0, 3);
    //    while (true)
    //    {
    //        switch (randomDirection)
    //        {
    //            case 0:
    //                {
    //                    Debug.Log("Something");
    //                    yield break;
    //                }
    //            case 1:
    //                {
    //                    yield break;

    //                }
    //            case 2: 
    //                {
    //                    yield break;
    //                }
    //            default:
    //                {
    //                    yield break;

    //                }

    //        }
    //        yield return new WaitForSeconds(2);
    //    }
    //}
}
[System.Serializable]
public enum PestType
{
    green,
    red
}
