using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerEventHandler
{
    public void GetDamage(int damage);
    public void ObtainItem(InventoryItem item);
    public void ObtainGold(int gold);
    public void MoveToTargetScene(string targetSceneName, PortalType type, int portalId = 0);
    public void MoveToNextSubStage(int subStageId);
    public void EncountMonster(Monster monster);
    public bool UsePaint();
    public void ShowEventDialog(string dialogId);
    public void ShowAdventureDialog(string dialogId, int dialogIndex = 0);
    public void UpdateMapProgress();
    public void SaveDataOnPurify(int portalId);
    public void EnableSelfCameraButton(bool isEnable);
    public void EnableSettingMenuButton(bool isEnable);

    public enum PortalType
    {
        DUNGEON,
        SANCTUARY
    }
}
