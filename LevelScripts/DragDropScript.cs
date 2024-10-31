using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private GameObject seedCopy;
    public Image image;

    //private Vector3 myPosition = transform.position;
    [HideInInspector] public Transform parentAfterDrag;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("On Pointer Down");
        seedCopy = Instantiate(gameObject);
        Debug.Log(seedCopy == null);

        seedCopy.transform.SetParent(seedCopy.transform.parent, false);

        RectTransform originalRect = GetComponent<RectTransform>();
        RectTransform cloneRect = seedCopy.GetComponent<RectTransform>();
        cloneRect.sizeDelta = originalRect.sizeDelta;

        cloneRect.position = Input.mousePosition;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter == null);
        Debug.Log("On Begin Drag");
        parentAfterDrag = seedCopy.transform.parent;
        seedCopy.transform.SetParent(transform.root);
        seedCopy.transform.SetAsLastSibling();

        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {

        Debug.Log("On Drag");
        Debug.Log(eventData.pointerEnter == null);
        seedCopy.transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {

        Debug.Log("On End Drag");
        Debug.Log(eventData.pointerEnter == null);
        //if (Input.mousePosition )
        // {

        // }    

        if (eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<PlotScript>() != null)
        {
            // Attach to the plot and keep the clone
            seedCopy.transform.SetParent(eventData.pointerEnter.transform);
            eventData.pointerEnter.GetComponent<PlotScript>().hasPlant = true; // Update plot status
        }
        else
        {
            // Destroy the clone if not placed on a plot
            Destroy(seedCopy);
        }
        seedCopy.transform.SetParent(parentAfterDrag, false);
        image.raycastTarget = true;
    }


}
