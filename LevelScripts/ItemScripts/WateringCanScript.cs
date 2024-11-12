using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WateringCanScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector] public GameObject dragCopy;
    public GameObject eventSystem;
    public int price = 0;

    //public Image image;
    private CanvasGroup canvasGroup;
    private PointerEventData _lastPointerData;


    //private Vector3 myPosition = transform.position;
    [HideInInspector] public Transform parentAfterDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {

        dragCopy = gameObject;

        StatsScript money = eventSystem.GetComponent<StatsScript>();

        if (money.moneyAvailable >= price)
        {

            canvasGroup = dragCopy.GetComponent<CanvasGroup>();

            if (dragCopy != null)
            {
                parentAfterDrag = dragCopy.transform.parent;
                //dragCopy.transform.SetParent(transform.root);
                //dragCopy.transform.SetAsLastSibling();

                canvasGroup.blocksRaycasts = false;
                canvasGroup.alpha = 0.5F;
            }

            //image.raycastTarget = false;
            _lastPointerData = eventData;

        }
        else
        {
            Debug.Log("Not enough money!");
            CancelDrag();

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
        if (dragCopy != null)
        {
            dragCopy.transform.position = parentAfterDrag.position;
            canvasGroup.alpha = 1F;
            canvasGroup.blocksRaycasts = true;
        }
    }
}