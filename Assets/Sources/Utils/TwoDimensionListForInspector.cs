using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TwoDimensionListForInspector<T>
{
    [SerializeField] private List<T> list;

    public List<T> GetList()
    {
        return list;
    }
}
