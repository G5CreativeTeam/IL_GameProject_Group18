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

    [Header("Properties")]
    public int price = 100;
    public float cooldownTime = 0;
    //public Image image;

    private CanvasGroup canvasGroup;
    private PointerEventData _lastPointerData;


    //private Vector3 myPosition = transform.position;
    [HideInInspector] public float cooldownTimer;
    [HideInInspector] public Transform parentAfterDrag;

    public void Start()
    {
        cooldownTimer = cooldownTime;
    }

    public void Update()
    {
        if (cooldownTimer < cooldownTime)
        {
            cooldownTimer += Time.deltaTime;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        dragCopy = Instantiate(gameObject, transform);

        StatsScript money = levelProperties.GetComponent<StatsScript>();
        

        if (money.moneyAvailable >= price && cooldownTimer >= cooldownTime)
        {

            canvasGroup = dragCopy.GetComponent<CanvasGroup>();

            //dragCopy.transform.SetParent(dragCopy.transform.parent, false);

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
                //cooldownTimer = 0;
            }

            //image.raycastTarget = false;
            _lastPointerData = eventData;

        }
        else
        {
            Debug.Log("Unable to plant!");
            Destroy(dragCopy);
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

            // Reset position here
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        StatsScript money = levelProperties.GetComponent<StatsScript>();
        

        if (dragCopy != null)
        {
            
            if (eventData.pointerEnter.GetComponent<PlotScript>() != null && eventData.pointerEnter.GetComponent<PlotScript>().hasPlant == false)
            {
                
                
                eventData.pointerEnter.GetComponent<PlotScript>().hasPlant = true;
            }
            Destroy(dragCopy);

            canvasGroup.alpha = 1F;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
