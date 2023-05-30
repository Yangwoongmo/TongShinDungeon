using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapPrototypeScene : MonoBehaviour
{
    private const string BattlePrototypeSceneName = "BattlePrototypeScene";
    [SerializeField] private MapPrototypeSceneController controller;

    public void OnForwardDown()
    {
        controller.OnForwardDown();
    }
    public void OnForwardUp()
    {
        controller.OnForwardUp();
    }

    public void OnLeftDown()
    {
        controller.OnRotateDown(-90);
    }
    public void OnLeftUp()
    {
        controller.OnRotateUp();
    }

    public void OnRightDown()
    {
        controller.OnRotateDown(90);
    }
    public void OnRightUp()
    {
        controller.OnRotateUp();
    }

    public void OnBattleClick()
    {
        SceneManager.LoadScene(BattlePrototypeSceneName);
    }

    public void OnPlayerCameraChangeClick()
    {
        controller.OnPlayerCameraChangeClick();
    }
}
