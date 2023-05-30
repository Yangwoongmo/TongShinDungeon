using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPortal : Portal
{
    [SerializeField] private int targetPortalId;
    [SerializeField] private bool isForNextLevel;
    [SerializeField] private int targetStageId;

    public override bool MoveToTargetArea(PlayerEventHandler eventHandler)
    {
        if (isForNextLevel)
        {
            eventHandler.MoveToTargetScene(targetSceneName, PlayerEventHandler.PortalType.DUNGEON, targetPortalId);
            MapObjectStatusManager.GetInstance().SetStageId(targetStageId);
        }
        else
        {
            eventHandler.MoveToNextSubStage(targetPortalId);
        }
        
        return true;
    }

    public void SetupPortalInfo(int targetStageId, int targetPortalId)
    {
#if UNITY_EDITOR
        this.targetStageId = targetStageId;
        if (targetStageId > 0)
        {
            targetSceneName = "MapStage" + targetStageId + "Scene";
        }
        else
        {
            targetSceneName = "TutorialScene";
        }
        this.targetPortalId = targetPortalId;
        isForNextLevel = true;
#endif
    }
}
