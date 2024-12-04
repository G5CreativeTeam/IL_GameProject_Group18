using System;
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

    [Header("Animation")]
    public float animSpeed;
    public Sprite[] animationFrames;

    [Header("Audio")]
    public GameObject attackAudio;

    public GameObject eventsystem;

    [HideInInspector] public GameObject target;
    [HideInInspector] public bool originalPest = true;
    [HideInInspector] public bool isCurrentlyColliding;

    private float attackTimer;
    private GameObject currentPlant; // Tracks the plant being attacked
    private List<GameObject> otherColliders = new List<GameObject>(); // Tracks other colliding objects
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

    public void Start()
    {
        temp = speed;
        rb = GetComponent<Rigidbody2D>();
        attackTimer = 0;
        FindNearestPlant();
        if (!originalPest)
        {
            StartCoroutine(Animate());
        }
    }

    public void Update()
    {
        if (isSpinning)
        {
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
        if (Input.GetMouseButtonDown(0) && !isSpinning)
        {
            swipeStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (GetComponent<Collider2D>().bounds.Contains(clickPosition))
            {
                isStopped = true;
                rb.velocity = Vector2.zero;
                StopCoroutine(Animate());
                isSwiping = true;
            }
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            swipeEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float swipeDistance = Vector2.Distance(swipeStart, swipeEnd);

            if (swipeDistance >= minSwipeDistance)
            {
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

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (!hasSwiped)
        {
            if (col.gameObject.TryGetComponent<PlantScript>(out PlantScript plant))
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
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.transform.position,
                eventsystem.GetComponent<LevelProperties>().pestSpeedMultiplier * speed * Time.deltaTime
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
            if (plant.isAlive && viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
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
        if (target == null || hasSwiped) return;

        if (target.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (target.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
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
}
