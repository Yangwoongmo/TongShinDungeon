using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerButtonClickListener
{
    public void OnRotateDown(float angle);
    public void OnRotateUp();
    public void OnForwardDown();
    public void OnForwardUp();
    public void OnPlayerCameraChangeClick(bool withoutEntrance = false);
    public void OnClickIntuition();
    public void OnClickResearch();
    public void OnClickDialogConfirm();
    
    // Only for debug action. We need to remove when prototyping is done. 
    public void OnResetDataClick();
}
