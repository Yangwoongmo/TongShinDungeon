using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneController : MonoBehaviour
{
    private void Awake()
    {
        StartPointDataManager manager = StartPointDataManager.GetInstance();
        StartPointData startPoint = manager.GetStartPoint();
        MapObjectStatusManager mapStatusManager = MapObjectStatusManager.GetInstance();

        if (startPoint != null && startPoint.GetStageId() > 0)
        {
            manager.SetNeedToSaveAfterTransitionDone(false);
            mapStatusManager.SetStageId(startPoint.GetStageId());
            SceneManager.LoadScene("MapStage" + startPoint.GetStageId() + "Scene");
        }
        else if (SanctuaryInfoRepository.GetInstance().HasSanctuarySceneVisited())
        {
            manager.SetNeedToSaveAfterTransitionDone(false);
            manager.SetInitialStartFloorLoadDone();
            manager.SetInitialBlinkPointLoadDone();
            SceneManager.LoadScene("SanctuaryScene");
        }
        else
        {
            MapObjectStatusManager.GetInstance().SetStageId(0);
            SceneManager.LoadScene("TutorialScene");
        }
    }
}
