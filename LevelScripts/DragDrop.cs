using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public Image image;
    public int seedNum;
    //private Vector3 myPosition = transform.position;
    [HideInInspector] public Transform parentAfterDrag;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (seedNum > 0)
        {
            Debug.Log("On Begin Drag");
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
        } else
        {
            Debug.Log("No seed left");
        }
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (seedNum > 0)
        {
            Debug.Log("On Drag");
            transform.position = Input.mousePosition;
            
        } else
        {
            Debug.Log("No seed left");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (seedNum > 0)
        {
            Debug.Log("On End Drag");
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
            seedNum--;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("On Pointer Down");
    }
}
