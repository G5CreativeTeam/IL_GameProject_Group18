using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlotScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            PlantScript draggableItem = dropped.GetComponent<PlantScript>();
            draggableItem.parentAfterDrag = transform;
        }
        
        
    }

}
