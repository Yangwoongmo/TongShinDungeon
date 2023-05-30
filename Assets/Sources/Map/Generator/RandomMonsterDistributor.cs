using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMonsterDistributor : MonoBehaviour
{
    [SerializeField] private MonsterGroup[] randomMonsterGroup;
    [SerializeField] private MonsterGroup[] fixedMonsterGroup;
    private MapObjectStatusManager manager = MapObjectStatusManager.GetInstance();

    public void ShuffleMonsterForBlink(int groupIndex, int spawnIndex)
    {
        if (groupIndex / 1000 > 0)
        {
            MonsterSpawn fixedSpawn = fixedMonsterGroup[groupIndex % 1000].GetSpawns()[0];
            if (!fixedSpawn.IsBossAttack())
            {
                return;
            }

            fixedSpawn.CleanupMonster();

            string id = fixedSpawn.transform.parent.GetComponent<Floor>().GetId();
            manager.UpdateObjectStatus(id);
            return;
        }

        MonsterSpawn currentSpawn = randomMonsterGroup[groupIndex].GetSpawns()[spawnIndex];
        MonsterGroup group = randomMonsterGroup[groupIndex];
        MonsterSpawn[] spawns = group.GetSpawns();
        List<MonsterSpawn> residualSpawns = new List<MonsterSpawn>();
        for (int i = 0; i < spawns.Length; i++)
        {
            if (i == spawnIndex)
            {
                continue;
            }
            if (!spawns[i].HasMonster())
            {
                residualSpawns.Add(spawns[i]);
            }
        }

        if (residualSpawns.Count == 0)
        {
            return;
        }
        int monsterId = currentSpawn.GetMonsterId();
        currentSpawn.CleanupMonster();

        MonsterSpawn targetSpawn = residualSpawns[Random.Range(0, residualSpawns.Count)];
        ActivateTargetMonsterSpawn(targetSpawn, monsterId);
    }

    public void DistributeMonsters()
    {
        for (int i = 0; i < randomMonsterGroup.Length; i++)
        {
            MonsterGroup monsterGroup = randomMonsterGroup[i];
            MonsterSpawn[] spawns = monsterGroup.GetSpawns();
            int[] monsterIds = monsterGroup.GetMonsterIds();

            List<int> indexPool = new List<int>();
            for (int j = 0; j < spawns.Length; j++)
            {
                indexPool.Add(j);
            }

            for (int k = 0; k < monsterIds.Length; k++)
            {
                if (indexPool.Count == 0)
                {
                    return;
                }
                int selectedIndex = Random.Range(0, indexPool.Count);
                MonsterSpawn selectedSpawn = spawns[indexPool[selectedIndex]];
                indexPool.RemoveAt(selectedIndex);

                ActivateTargetMonsterSpawn(selectedSpawn, monsterIds[k]);
            }
        }

        for (int i = 0; i < fixedMonsterGroup.Length; i++)
        {
            string id = fixedMonsterGroup[i].GetSpawns()[0].transform.parent.GetComponent<Floor>().GetId();
            if (manager.HasObjectStateChanged(id))
            {
                continue;
            }

            ActivateTargetMonsterSpawn(fixedMonsterGroup[i].GetSpawns()[0], fixedMonsterGroup[i].GetMonsterIds()[0]);
        }
    }

    public void CleanupAllSpawans()
    {
        for (int i = 0; i < randomMonsterGroup.Length; i++)
        {
            MonsterGroup monsterGroup = randomMonsterGroup[i];
            MonsterSpawn[] spawns = monsterGroup.GetSpawns();
            
            for (int j = 0; j < spawns.Length; j++)
            {
                spawns[j].CleanupMonster();
            }
        }
    }

    public void ClearSpawns(int randomSize, int fixedSize)
    {
#if UNITY_EDITOR
        randomMonsterGroup = new MonsterGroup[randomSize];
        fixedMonsterGroup = new MonsterGroup[fixedSize];
#endif
    }

    public void AddMonsterGroup(
        int groupId, 
        bool isFixedMonster, 
        int[] monsterIds, 
        MonsterSpawn[] spawns
    )
    {
#if UNITY_EDITOR
        if (isFixedMonster)
        {
            fixedMonsterGroup[groupId] = new MonsterGroup(spawns, monsterIds);
        }
        else
        {
            randomMonsterGroup[groupId] = new MonsterGroup(spawns, monsterIds);
        }
#endif
    }

    private void ActivateTargetMonsterSpawn(MonsterSpawn target, int monsterId)
    {
        target.SetMonsterId(monsterId);
        target.gameObject.SetActive(true);
        target.gameObject.transform.parent.GetComponent<Floor>().SetMonsterFloor(true);
    }

    private void Awake()
    {
        SetIndexToMonsterSpawns();
    }

    private void SetIndexToMonsterSpawns()
    {
        for (int i = 0; i < randomMonsterGroup.Length; i++)
        {
            MonsterGroup group = randomMonsterGroup[i];
            MonsterSpawn[] spawns = group.GetSpawns();
            for (int j = 0; j < spawns.Length; j++)
            {
                MonsterSpawn spawn = spawns[j];
                spawn.SetGroupIndex(i);
                spawn.SetSpawnIndex(j);
            }
        }

        for (int i = 0; i < fixedMonsterGroup.Length; i++)
        {
            MonsterGroup group = fixedMonsterGroup[i];
            MonsterSpawn spawn = group.GetSpawns()[0];
            spawn.SetGroupIndex(1000 + i);
            spawn.SetSpawnIndex(0);
        }
    }
}

[System.Serializable]
public class MonsterGroup
{
    [SerializeField] private MonsterSpawn[] spawns;
    [SerializeField] private int[] monsterIds;

    public MonsterGroup(MonsterSpawn[] spawns, int[] monsterIds)
    {
        this.spawns = new MonsterSpawn[spawns.Length];
        for (int i = 0; i < spawns.Length; i++)
        {
            this.spawns[i] = spawns[i];
        }

        this.monsterIds = new int[monsterIds.Length];
        for (int i = 0; i < monsterIds.Length; i++)
        {
            this.monsterIds[i] = monsterIds[i];
        }
    }

    public MonsterSpawn[] GetSpawns()
    {
        return spawns;
    }

    public int[] GetMonsterIds()
    {
        return monsterIds;
    }
}
