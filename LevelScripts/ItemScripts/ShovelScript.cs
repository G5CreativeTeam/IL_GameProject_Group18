using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShovelScript : MonoBehaviour, IPointerClickHandler
{
    public GameObject eventSystem;
    public int price = 0;
    [SerializeField] private KeyCode shortcutKey = KeyCode.W;

    private bool isFollowingMouse = false;
    private CanvasGroup canvasGroup;
    private StatsScript money;

    private Vector3 initialPosition; // Store the initial position of the fertilizer

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        money = eventSystem.GetComponent<StatsScript>();
        initialPosition = transform.position; // Save the initial position
    }

    private void Update()
    {
        if (!LevelProperties.Instance.isCarryingObject && Input.GetKeyDown(shortcutKey))
        {
            AttemptPickup();
        }
        if (isFollowingMouse)
        {
            FollowMouse();
        }
    }

    private void FollowMouse()
    {
        // Make the fertilizer follow the mouse position
        transform.position = Input.mousePosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (!LevelProperties.Instance.isCarryingObject && !isFollowingMouse)
        {
            AttemptPickup();
            Debug.Log("Check 1");
        }
        else
        {
            DropShovel();
            Debug.Log("Check 2");
        }
    }

    private void AttemptPickup()
    {
        if (money.moneyAvailable >= price)
        {
            isFollowingMouse = true;
            LevelProperties.Instance.isCarryingObject = true;
            canvasGroup.alpha = 0.5f;
            canvasGroup.blocksRaycasts = true; // Allow interaction during dragging
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    private void DropShovel()
    {
        // Convert mouse position to world position
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Perform a 2D raycast
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);
        Debug.Log("Check");
        if (hit.collider != null)
        {
            // Check if the hit object has the PlotScript component
            PlotScript plot = hit.collider.GetComponent<PlotScript>();
            if (plot != null && plot.hasPlant)
            {
                Debug.Log("Dropped on a valid plot!");
                LevelProperties.Instance.isCarryingObject = false;

                // Reset dragging state
                isFollowingMouse = false;
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
                plot.ShovelDrop();
                // Handle successful drop logic here (e.g., planting, deducting money, etc.)
                // Exit to avoid resetting position
            }
            else
            {
                Debug.Log("Not a valid plot.");
            }
        }

        // Reset the fertilizer to its original position if not dropped on a valid plot
        transform.position = initialPosition;
        isFollowingMouse = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        LevelProperties.Instance.isCarryingObject = false;

        Debug.Log("Fertilizer returned to initial position.");
    }

}