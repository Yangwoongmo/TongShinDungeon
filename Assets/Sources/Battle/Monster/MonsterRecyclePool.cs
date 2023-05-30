using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRecyclePool : MonoBehaviour
{
    [SerializeField] private SerializableMap<int, Transform> monsterMap;

    public GameObject GetMonster(int id, Transform target)
    {
        Transform parent = monsterMap.GetValue(id);
        if (parent == null)
        {
            return null;
        }

        if (parent.childCount > 0)
        {
            Transform monster = parent.GetChild(0);
            monster.SetParent(target, false);
            
            return monster.gameObject;
        } 
        else
        {
            return Instantiate((GameObject)Resources.Load("Monster/monster" + id), target);
        }
    }

    public void RecycleMonster(int id, GameObject monster)
    {
        Transform parent = monsterMap.GetValue(id);
        if (parent == null)
        {
            return;
        }

        monster.SetActive(false);
        monster.transform.SetParent(parent, false);
    }
}
