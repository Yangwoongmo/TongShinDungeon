using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * When use this to inspector view, we should set key in order,
 * since we use binary search to find key index.
 */
[System.Serializable]
public class SerializableMap<TKey, TValue>
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public TValue GetValue(TKey key)
    {
        int idx = GetKeyIndex(key);
        if (idx < 0)
        {
            return default;
        } 
        else
        {
            return values[idx];
        }
    }

    public void Add(TKey key, TValue value)
    {
        int idx = GetKeyIndex(key);
        if (idx < 0)
        {
            keys.Add(key);
            values.Add(value);
        }
        else
        {
            values[idx] = value;
        }
    }

    private int GetKeyIndex(TKey key)
    {
        return keys.BinarySearch(key);
    }
}
