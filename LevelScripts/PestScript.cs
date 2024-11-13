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

    public void Start()
    {
        attackTimer = 0;
        FindNearestPlant();
    }

    public void Update()
    {
        target = FindNearestPlant();
        if (newSpawn && Camera.main.WorldToViewportPoint(gameObject.transform.position).x > 0)
        {
            newSpawn = false;
            
        }
            if (target != null && !originalPest && gameObject)
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
        
        
        
        
    }

    
    public void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("CHeck");
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

            // Check if the plant is within the screen bounds (visible in the camera view)
            if ((viewportPos.x >= 0 && viewportPos.x <= 1) && (viewportPos.y >= 0 && viewportPos.y <= 1 ))
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

    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedPest = gameObject;
        canvasGroup = draggedPest.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        Debug.Log("Drag starts!");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedPest.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, transform.position.z);
        Debug.Log("Ended");
        canvasGroup.blocksRaycasts = true;
        if (Camera.main.WorldToViewportPoint(gameObject.transform.position).x < 0 && !newSpawn)
        {
            PestDeath();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedPest != null)
        {
            draggedPest.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y,transform.position.z);
            
        }
        else
        {
            CancelDrag();
            Debug.Log("You're cancelled");
        }
    }

    public void CancelDrag()
    {
        if (_lastPointerData != null)
        {
            _lastPointerData.pointerDrag = null;

            // Reset position here
        }
    }

    public void PestDeath()
    {
        Destroy(gameObject);
        eventsystem.GetComponent<EventSystem>().numOfPests--;
    }
}
