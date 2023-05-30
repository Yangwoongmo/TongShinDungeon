using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePreferenceManager : MonoBehaviour
{
    private GamePreferenceManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } 
        else
        {
            Destroy(this.gameObject);
        }

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    public GamePreferenceManager getInstance()
    {
        return instance;
    }
}
