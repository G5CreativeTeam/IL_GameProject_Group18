using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlantScript: MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public Image image;
  
    //private Vector3 myPosition = transform.position;
    [HideInInspector] public Transform parentAfterDrag;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
   
            Debug.Log("On Begin Drag");
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
      }

    public void OnDrag(PointerEventData eventData)
    {

            Debug.Log("On Drag");
            transform.position = Input.mousePosition;                  
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        
            Debug.Log("On End Drag");
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("On Pointer Down");
    }
}
