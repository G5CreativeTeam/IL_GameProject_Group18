using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlotScript : MonoBehaviour, IDropHandler
{
    public bool hasPlant = false;
    public void OnDrop(PointerEventData eventData)
    {
        hasPlant = transform.childCount != 0;
        if (!hasPlant)
        {
            GameObject dropped = eventData.pointerDrag;
            
            DragDrop draggableItem = dropped.GetComponent<DragDrop>();
            
            draggableItem.parentAfterDrag = transform;
            hasPlant = true;
        }
        
        
    }

}
