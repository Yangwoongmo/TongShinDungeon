using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SanctuaryController : MonoBehaviour
{
    protected bool isButtonClickAvailable = false;
    protected Action saveDataCallback;

    public virtual void SetEnable(bool isEnabled)
    {
        isButtonClickAvailable = false;
    }

    public virtual void SetPlayer(Player player)
    {
        // Do nothing at default.
    }

    public void SetButtonClickAvailable(bool isAvailable)
    {
        isButtonClickAvailable = isAvailable;
    }

    public virtual void SetSaveDataCallback(Action saveDataCallback)
    {
        this.saveDataCallback = saveDataCallback;
    }
}
