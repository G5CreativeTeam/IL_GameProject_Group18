using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SeedScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private GameObject dragCopy;
    [Header("Related Objects")]
    public GameObject levelProperties;
    public GameObject plant;
    public GameObject mask;

    [Header("Properties")]
    public int price = 100;
    public float cooldownTime = 0;

    private CanvasGroup canvasGroup;
    private PointerEventData _lastPointerData;
    private float percentage;
    private int emptyCoordinates = 45;
    private bool isDragging = false; // Flag to indicate drag state

    [HideInInspector] public float cooldownTimer;
    [HideInInspector] public Transform parentAfterDrag;

    public void Start()
    {
        cooldownTimer = cooldownTime;
        percentage = cooldownTimer / cooldownTime;
        if (mask != null)
        {
            mask.GetComponent<RectTransform>().offsetMax = new Vector2(0, -percentage * emptyCoordinates);
        }
        
    }

    public void Update()
    {
        // Only update the mask during cooldown, not when dragging
        if (!isDragging && cooldownTimer < cooldownTime)
        {
            cooldownTimer += Time.deltaTime;
            percentage = cooldownTimer / cooldownTime;

            mask.GetComponent<RectTransform>().offsetMax = new Vector2(0, -percentage * emptyCoordinates);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true; // Set dragging flag

        dragCopy = Instantiate(gameObject, transform);
        dragCopy.GetComponent<SeedScript>().mask = null;
        StatsScript money = levelProperties.GetComponent<StatsScript>();

        if (money.moneyAvailable >= price && cooldownTimer >= cooldownTime)
        {
            RectTransform maskRect = mask.GetComponent<RectTransform>();
            maskRect.offsetMax = new Vector2(maskRect.offsetMax.x, 0); // Set mask to full
            canvasGroup = dragCopy.GetComponent<CanvasGroup>();

            RectTransform originalRect = GetComponent<RectTransform>();
            RectTransform cloneRect = dragCopy.GetComponent<RectTransform>();
            cloneRect.sizeDelta = originalRect.sizeDelta;
            cloneRect.position = Input.mousePosition;

            if (dragCopy != null)
            {
                parentAfterDrag = dragCopy.transform.parent;
                dragCopy.transform.SetParent(transform.root);
                dragCopy.transform.SetAsLastSibling();

                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0.5F;
            }

            _lastPointerData = eventData;
        }
        else
        {
            Debug.Log("Unable to plant!");
            Destroy(dragCopy);
            isDragging = false; // Reset flag if dragging fails
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragCopy != null)
        {
            dragCopy.transform.position = Input.mousePosition;
        }
        else
        {
            CancelDrag();
            eventData = null;
        }
    }

    public void CancelDrag()
    {
        if (_lastPointerData != null)
        {
            _lastPointerData.pointerDrag = null;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false; // Reset dragging flag
        //StatsScript money = levelProperties.GetComponent<StatsScript>();

        if (dragCopy != null)
        {
            if (eventData.pointerEnter.GetComponent<PlotScript>() != null &&
                !eventData.pointerEnter.GetComponent<PlotScript>().hasPlant)
            {
                eventData.pointerEnter.GetComponent<PlotScript>().hasPlant = true;
            }
            else
            {
                mask.GetComponent<RectTransform>().offsetMax = new Vector2(0, -percentage * emptyCoordinates);
            }

            Destroy(dragCopy);

            canvasGroup.alpha = 1F;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
