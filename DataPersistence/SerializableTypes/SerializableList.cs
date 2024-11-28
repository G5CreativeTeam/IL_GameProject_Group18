using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableList<plotData> : List<plotData>, ISerializationCallbackReceiver
{

    [SerializeField] private List<plotData> list = new List<plotData>();
    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            this.Add(list[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        list.Clear();
        foreach (plotData item in this) 
        {
            list.Add(item);
        }
    }
}
