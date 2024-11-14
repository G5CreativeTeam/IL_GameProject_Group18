using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PestScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public int maxHealth = 10;
    public float attackRate = 2;
    public int damage = 2;
    public int speed = 3;
    public int size = 1;
    public float dragForceMultiplier = 500f;
    public float dragDamping = 0.98f;

    public GameObject target;
    public GameObject eventsystem;

    [HideInInspector] public bool originalPest = true;
    [HideInInspector] public bool isCurrentlyColliding;

    private PointerEventData _lastPointerData;
    private GameObject draggedPest;
    private CanvasGroup canvasGroup;
    private float distance;
    private bool newSpawn = true;
    private float attackTimer;
    private Collision2D collidedObject;
    private bool isDragged = false;

    private Vector3 lastPosition;
    private Vector3 throwDirection;
    private Rigidbody2D rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackTimer = 0;
        FindNearestPlant();
    }

    public void Update()
    {
        target = FindNearestPlant();
        if (newSpawn && !PestOutOfScreen())
        {
            newSpawn = false;
        }
        if (target != null && !originalPest && !isDragged)
        {
            TowardsPlant(target);
        }
        if (isCurrentlyColliding)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                collidedObject.gameObject.GetComponent<PlantScript>().TakeDamage(damage);
                attackTimer = 0;
            }
        }
        if (PestOutOfScreen() && !newSpawn)
        {
            
            PestDeath();
        }

        // Gradually decrease the speed of the throw by applying damping to the velocity
        rb.velocity *= dragDamping;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        
        collidedObject = col;
        isCurrentlyColliding = true;
    }

    public void OnCollisionExit2D(Collision2D col)
    {
        isCurrentlyColliding = false;
        collidedObject = null;
    }

    public void TowardsPlant(GameObject target)
    {
        distance = Vector2.Distance(transform.position, target.transform.position);

        if (target != null)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, target.transform.position, (eventsystem.GetComponent<EventSystem>().pestSpeedMultiplier * speed) * Time.deltaTime);
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

    public void AttackPlant()
    {
        // Placeholder for AttackPlant logic
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedPest = gameObject;
        canvasGroup = draggedPest.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        Debug.Log("Drag starts!");

        lastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastPosition.z = transform.position.z;
        isDragged = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        endPosition.z = transform.position.z;
        throwDirection = (endPosition - lastPosition).normalized;

        rb.AddForce(throwDirection * dragForceMultiplier);

        
        isDragged = false;


        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedPest != null)
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePosition.z = transform.position.z;
            draggedPest.transform.position = currentMousePosition;

            lastPosition = currentMousePosition;
        }
        else
        {
            CancelDrag();
   
        }
    }

    public void CancelDrag()
    {
        if (_lastPointerData != null)
        {
            _lastPointerData.pointerDrag = null;
        }
    }

    public void PestDeath()
    {
        Destroy(gameObject);
        eventsystem.GetComponent<EventSystem>().numOfPests--;
       
    }

    public bool PestOutOfScreen()
    {
        Bounds bounds = GetComponent<Collider2D>().bounds;
        Camera mainCamera = Camera.main;

        Vector3 minViewport = mainCamera.WorldToViewportPoint(bounds.min);
        Vector3 maxViewport = mainCamera.WorldToViewportPoint(bounds.max);

        return maxViewport.x < 0 || minViewport.x > 1 || maxViewport.y < 0 || minViewport.y > 1;
    }
}
