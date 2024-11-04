using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private GameObject dragCopy;
    public GameObject eventSystem;

    
    //public Image image;
    private CanvasGroup canvasGroup;
    private PointerEventData _lastPointerData;


    //private Vector3 myPosition = transform.position;
    [HideInInspector] public Transform parentAfterDrag;

    public void OnPointerDown(PointerEventData eventData)
    {
        dragCopy = Instantiate(gameObject);

        StatsScript money = eventSystem.GetComponent<StatsScript>();
        int itemPrice = dragCopy.GetComponent<itemScript>().price;

        if (dragCopy.GetComponent<itemScript>() != null && money.moneyAvailable >= itemPrice)
        {
            
            canvasGroup = dragCopy.GetComponent<CanvasGroup>();

            dragCopy.transform.SetParent(dragCopy.transform.parent, false);

            RectTransform originalRect = GetComponent<RectTransform>();
            RectTransform cloneRect = dragCopy.GetComponent<RectTransform>();
            cloneRect.sizeDelta = originalRect.sizeDelta;
            cloneRect.position = Input.mousePosition;

            
        } else
        {
            Debug.Log("Not enough money!");
            Destroy(dragCopy);
            
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {

        if (dragCopy != null)
        {
            parentAfterDrag = dragCopy.transform.parent;
            dragCopy.transform.SetParent(transform.root);
            dragCopy.transform.SetAsLastSibling();

            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.5F;
        }

        //image.raycastTarget = false;
        _lastPointerData = eventData;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragCopy != null)
        {
            dragCopy.transform.position = Input.mousePosition;
        } else
        {
            CancelDrag();
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
        StatsScript money = eventSystem.GetComponent<StatsScript>();
        int itemPrice = dragCopy.GetComponent<itemScript>().price;

        if (dragCopy != null)
        {
            
            if (eventData.pointerEnter.GetComponent<PlotScript>() != null && eventData.pointerEnter.GetComponent<PlotScript>().hasPlant == false)
            {
                if (dragCopy.GetComponent<itemScript>().plant != null)
                {
                    Debug.Log("Planted succesfully");
                    money.moneyAvailable = money.deductAmount(itemPrice,money.moneyAvailable);
                    eventData.pointerEnter.GetComponent<PlotScript>().hasPlant = true;
                } 
            }
            Destroy(dragCopy);

            
            canvasGroup.alpha = 1F;
            canvasGroup.blocksRaycasts = true;
        }
    }


}
