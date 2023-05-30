using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISettingMenuClickListener
{
    public void OnClickOpenSettingMenu();

    public void OnClickCloseSettingMenu();

    public void OnClickSaveData();

    public void OnClickSaveDataAndQuit();
}
