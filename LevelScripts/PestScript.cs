using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PestScript : MonoBehaviour
{
    [Header("Properties")]
    public int maxHealth = 10;
    public float attackRate = 2;
    public int damage = 2;
    public float speed = 3;
    public int size = 1;
    public float swipeSpeed = 5f;
    public float minSwipeDistance = 0.5f; // Minimum distance to qualify as a swipe

    [Header("Audio")]
    public GameObject attackAudio;

    public GameObject eventsystem;

    [HideInInspector] public GameObject target;
    [HideInInspector] public bool originalPest = true;
    [HideInInspector] public bool isCurrentlyColliding;

    private float attackTimer;
    private Collision2D collidedObject;
    private Rigidbody2D rb;

    private Vector2 swipeStart;
    private Vector2 swipeEnd;
    private bool isSwiping = false;
    private bool hasSwiped = false;
    private bool newSpawn = true;
    private bool isStopped = false; // Tracks if the pest is stopped
    private bool isSpinning = false; // Tracks if the pest is spinning
    private float spinDirection = 1f; // 1 for clockwise, -1 for counterclockwise
    private float temp ;

    public void Start()
    {
        temp = speed;
        rb = GetComponent<Rigidbody2D>();
        attackTimer = 0;
        FindNearestPlant();
    }

    public void Update()
    {

        if (isSpinning)
        {
            // Rotate the pest indefinitely based on spinDirection
            transform.Rotate(Vector3.forward * 360 * spinDirection * Time.deltaTime);
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
        
        if (isCurrentlyColliding && collidedObject != null )
        {   
            if (collidedObject.gameObject.TryGetComponent<PlantScript>(out PlantScript plant))
            {
                attackTimer += Time.deltaTime;
                
                //isStopped = true; // Stop the pest when found plant
                //temp = speed;
                //StopMoving();
                if (attackTimer >= attackRate)
                {
                    attackAudio.GetComponent<AudioSource>().Play();
                    StartCoroutine(plant.TakeDamage(damage));
                    attackTimer = 0;
                }
            }
            
        } else
        {

            //speed = temp;
            //Debug.Log("Moving");
        }

        
        
        if (!newSpawn)
        {
            
            HandleSwipe();
        }

        FlipSprite();
    }

    private void StopMoving()
    {
        speed = 0;
        isStopped = true;
    }

    private void HandleSwipe()
    {
       
        if (Input.GetMouseButtonDown(0) && !isSpinning)
        {
            swipeStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // Check if the click is on the pest
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (GetComponent<Collider2D>().bounds.Contains(clickPosition))
            {
                isStopped = true; // Stop the pest when clicked
                rb.velocity = Vector2.zero; // Stop its movement
                isSwiping = true; // Allow for a potential swipe
            }
        }
        if (Input.GetMouseButtonUp(0) && isSwiping)
        {

            swipeEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float swipeDistance = Vector2.Distance(swipeStart, swipeEnd);

            if (swipeDistance >= minSwipeDistance)
            {
                // This is a valid swipe
                Vector2 swipeDirection = (swipeEnd - swipeStart).normalized;

                // Set spin direction based on swipe
                spinDirection = swipeDirection.x > 0 ? -1f : 1f;

                // Apply swipe velocity
                rb.velocity = swipeDirection * swipeSpeed;

                // diable box collider to ignore collisions
                GetComponent<BoxCollider2D>().enabled = false;

                hasSwiped = true; // Pest no longer targets plants after being swiped

                // Start spinning after swipe
                isSpinning = true;
            }
            else
            {
                // Not a swipe, resume movement
                isStopped = false;
            }

            isSwiping = false; // Reset swiping state
        }
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (!hasSwiped)
        {
            collidedObject = col;
            isCurrentlyColliding = true;
        }
    }

    public void OnCollisionExit2D(Collision2D col)
    {
       
        isCurrentlyColliding = false;
        collidedObject = null;
        attackTimer = 0;
        
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
            transform.position = Vector2.MoveTowards(
                this.transform.position,
                target.transform.position,
                (eventsystem.GetComponent<LevelProperties>().pestSpeedMultiplier * speed) * Time.deltaTime
            );
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

            if ((viewportPos.x >= 0 && viewportPos.x <= 1) && (viewportPos.y >= 0 && viewportPos.y <= 1))
            {
                float distance = Vector2.Distance(transform.position, plant.transform.position);
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
        eventsystem.GetComponent<StatsScript>().numOfPests--;
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
        if (target == null || hasSwiped) return; // Skip flipping if already swiped

        if (target.transform.position.x < transform.position.x)
        {
            // Target is to the right
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (target.transform.position.x > transform.position.x)
        {
            // Target is to the left
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}
