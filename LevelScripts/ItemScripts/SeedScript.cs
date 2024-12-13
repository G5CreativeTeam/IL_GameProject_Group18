using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeedScript : MonoBehaviour, IPointerClickHandler/*, IBeginDragHandler, IEndDragHandler, IDragHandler */
{
    [Header("Related Objects")]
    public GameObject plant;
    public GameObject mask;

    [Header("Properties")]
    public int price = 100;
    public float cooldownTime = 0;

    private float percentage;
    private readonly int emptyCoordinates = 45;
    [SerializeField] private KeyCode shortcutKey = KeyCode.Alpha1;

    [HideInInspector] public float cooldownTimer;
    [HideInInspector] public bool availableToClick;
    [HideInInspector] public bool isFollowingMouse = false;

    private StatsScript money;
    private GameObject seedCopy;
    [HideInInspector] public GameObject originalSeed;
    private bool isOriginalSeed = true;

    //private Vector3 initialPosition; // Store the initial position of the fertilizer

    private void Start()
    {
        cooldownTimer = cooldownTime;
        availableToClick = true;

        cooldownTimer = cooldownTime;
        percentage = cooldownTimer / cooldownTime;
        if (mask != null)
        {
            mask.GetComponent<RectTransform>().offsetMax = new Vector2(0, -percentage * emptyCoordinates);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(shortcutKey) && !LevelProperties.Instance.isCarryingObject && availableToClick && isOriginalSeed)
        {
            AttemptPickup();
        } 
        if (Input.GetKeyDown(shortcutKey) 
            && LevelProperties.Instance.isCarryingObject 
            && !isOriginalSeed)
        {
            LevelProperties.Instance.isCarryingObject = false;
            LevelProperties.Instance.objectCarried = null;

            isFollowingMouse = false;
            Destroy(gameObject);
            if (originalSeed != null)
            {
                originalSeed.GetComponent<SeedScript>().mask.
        GetComponent<RectTransform>().offsetMax = new Vector2(0, -45);
            }
        }

        if (isFollowingMouse)
        {
            FollowMouse();
        }
        if (availableToClick)
        {
            gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }

        // Only update the mask during cooldown, not when dragging
        if (cooldownTimer < cooldownTime && mask != null)
        {
            cooldownTimer += Time.deltaTime;
            percentage = cooldownTimer / cooldownTime;

            mask.GetComponent<RectTransform>().offsetMax = new Vector2(0, -percentage * emptyCoordinates);
        }
    }

    private void FollowMouse()
    {
        if (seedCopy != null)
        {
            seedCopy.transform.position = Input.mousePosition;
        } else
        {
            isFollowingMouse = false;
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (!LevelProperties.Instance.isCarryingObject && !isFollowingMouse)
        {
            AttemptPickup();
        }
        else
        {
            DropSeed();
        }
    }

    private void AttemptPickup()
    {
        if (LevelProperties.Instance.GetComponent<StatsScript>().moneyAvailable >= price && cooldownTimer >= cooldownTime)
        {
            seedCopy = Instantiate(gameObject, transform.parent);
            seedCopy.GetComponent<SeedScript>().seedCopy = null;
            seedCopy.GetComponent<SeedScript>().mask = null;
            seedCopy.GetComponent<SeedScript>().originalSeed = gameObject;
            seedCopy.GetComponent<SeedScript>().isOriginalSeed = false;
            isFollowingMouse = true;

            LevelProperties.Instance.isCarryingObject = true;
            LevelProperties.Instance.objectCarried = gameObject;
            seedCopy.GetComponent<CanvasGroup>().alpha = 0.8f;
            seedCopy.GetComponent<CanvasGroup>().blocksRaycasts = true; // Allow interaction during dragging

            mask.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    private void DropSeed()
    {
        // Convert mouse position to world position
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Perform a 2D raycast
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            // Check if the hit object has the PlotScript component
            PlotScript plot = hit.collider.GetComponent<PlotScript>();
            if (plot != null && !plot.hasPlant && cooldownTime == cooldownTimer)
            {
                Debug.Log("Valid plot");
                
                // Reset dragging state
                plot.SeedDrop(gameObject.GetComponent<SeedScript>());
                

                if (originalSeed != null)
                {
                    originalSeed.GetComponent<SeedScript>().mask.
            GetComponent<RectTransform>().offsetMax = new Vector2(0, -45);
                }
            } else
            {
                if (originalSeed != null)
                {
                    originalSeed.GetComponent<SeedScript>().mask.
            GetComponent<RectTransform>().offsetMax = new Vector2(0, -45);
                }
            }
        } else
        {
            if (originalSeed != null)
            {
                originalSeed.GetComponent<SeedScript>().mask.
        GetComponent<RectTransform>().offsetMax = new Vector2(0, -45);
            }
        }
        LevelProperties.Instance.isCarryingObject = false;
        LevelProperties.Instance.objectCarried = null;

        Debug.Log("Check?");
        // Reset the fertilizer to its original position if not dropped on a valid plot
        isFollowingMouse = false;
        if (!isOriginalSeed)
        {
            Destroy(gameObject);
        }
        
        
    }

    // Old Code
    //public void ReturnToPosition()
    //{

    //    transform.position = initialPosition;
    //    isFollowingMouse = false;
    //    canvasGroup.alpha = 1f;
    //    canvasGroup.blocksRaycasts = true;
    //    LevelProperties.Instance.isCarryingObject = false;
    //    LevelProperties.Instance.objectCarried = null;
    //    availableToClick = true;
    //}

    //public void Start()
    //{
    //    cooldownTimer = cooldownTime;
    //    percentage = cooldownTimer / cooldownTime;
    //    if (mask != null)
    //    {
    //        mask.GetComponent<RectTransform>().offsetMax = new Vector2(0, -percentage * emptyCoordinates);
    //    }

    //}

    //public void Update()
    //{
    //    // Only update the mask during cooldown, not when dragging
    //    if (!isDragging && cooldownTimer < cooldownTime)
    //    {
    //        cooldownTimer += Time.deltaTime;
    //        percentage = cooldownTimer / cooldownTime;

    //        mask.GetComponent<RectTransform>().offsetMax = new Vector2(0, -percentage * emptyCoordinates);
    //    }
    //}

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    isDragging = true; // Set dragging flag

    //    dragCopy = Instantiate(gameObject, transform);
    //    dragCopy.GetComponent<SeedScript>().mask = null;
    //    StatsScript money = levelProperties.GetComponent<StatsScript>();

    //    if (money.moneyAvailable >= price && cooldownTimer >= cooldownTime)
    //    {
    //        RectTransform maskRect = mask.GetComponent<RectTransform>();
    //        maskRect.offsetMax = new Vector2(maskRect.offsetMax.x, 0); // Set mask to full
    //        canvasGroup = dragCopy.GetComponent<CanvasGroup>();

    //        RectTransform originalRect = GetComponent<RectTransform>();
    //        RectTransform cloneRect = dragCopy.GetComponent<RectTransform>();
    //        cloneRect.sizeDelta = originalRect.sizeDelta;
    //        cloneRect.position = Input.mousePosition;

    //        if (dragCopy != null)
    //        {
    //            parentAfterDrag = dragCopy.transform.parent;
    //            dragCopy.transform.SetParent(transform.root);
    //            dragCopy.transform.SetAsLastSibling();

    //            canvasGroup.blocksRaycasts = false;
    //            canvasGroup.alpha = 0.5F;
    //        }

    //        _lastPointerData = eventData;
    //    }
    //    else
    //    {
    //        Debug.Log("Unable to plant!");
    //        Destroy(dragCopy);
    //        isDragging = false; // Reset flag if dragging fails
    //    }
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    if (dragCopy != null)
    //    {
    //        dragCopy.transform.position = Input.mousePosition;
    //    }
    //    else
    //    {
    //        CancelDrag();
    //        eventData = null;
    //    }
    //}

    //public void CancelDrag()
    //{
    //    if (_lastPointerData != null)
    //    {
    //        _lastPointerData.pointerDrag = null;
    //    }
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    isDragging = false; // Reset dragging flag
    //    //StatsScript money = levelProperties.GetComponent<StatsScript>();

    //    if (dragCopy != null)
    //    {
    //        if (eventData.pointerEnter.GetComponent<PlotScript>() != null &&
    //            !eventData.pointerEnter.GetComponent<PlotScript>().hasPlant)
    //        {
    //            eventData.pointerEnter.GetComponent<PlotScript>().hasPlant = true;
    //        }
    //        else
    //        {
    //            mask.GetComponent<RectTransform>().offsetMax = new Vector2(0, -percentage * emptyCoordinates);
    //        }

    //        Destroy(dragCopy);

    //        canvasGroup.alpha = 1F;
    //        canvasGroup.blocksRaycasts = true;
    //    }
    //}
}
