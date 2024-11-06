using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PestScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public int maxHealth = 10;
    public int damage = 2;
    public int speed = 3;
    public int size = 1;

    public GameObject target;
    private float distance;

    public void Start()
    {
        FindNearestPlant();
    }

    public void Update()
    {
        target = FindNearestPlant();
        if (target != null)
        {
            TowardsPlant(target);
        }
        
        
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided");
        if (collision.gameObject.CompareTag("Plant"))
        {
            Debug.Log("Ow!");
            collision.gameObject.GetComponent<PlantScript>().TakeDamage(damage);
        }
    }

    public void TowardsPlant(GameObject target)
    {
        distance = Vector2.Distance(transform.position, target.transform.position);
        Vector2 direction = target.transform.position - transform.position;
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, target.transform.position, speed * Time.deltaTime);
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

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
