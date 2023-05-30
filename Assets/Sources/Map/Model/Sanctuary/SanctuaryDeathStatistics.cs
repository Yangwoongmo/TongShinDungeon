using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SanctuaryDeathStatistics
{
    [SerializeField] private int deathCount;
    [SerializeField] private int progress;

    public SanctuaryDeathStatistics(int deathCount, int progress)
    {
        this.deathCount = deathCount;
        this.progress = progress;
    }

    public void IncreaseDeathCount()
    {
        deathCount++;
    }

    public void IncreaseProgress()
    {
        progress++;
    }

    public int GetDeathCount()
    {
        return deathCount;
    }

    public int GetProgress()
    {
        return progress;
    }
}
