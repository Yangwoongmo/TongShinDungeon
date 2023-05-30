using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogStatusListener
{
    public void OnDialogComplete();
    public void OnSelfCameraModeDialogComplete(bool needToRotateCamera = false);
}
