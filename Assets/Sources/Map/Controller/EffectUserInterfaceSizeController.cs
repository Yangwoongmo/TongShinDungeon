using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectUserInterfaceSizeController : MonoBehaviour
{
    [SerializeField] private RectTransform effectUICanvas;
    [SerializeField] private RectTransform cameraChangeEffectUI;

    void Start()
    {
        SetCameraChangeEffectUISize();
    }

    private void SetCameraChangeEffectUISize()
    {
        int screenSizeX = (int)effectUICanvas.sizeDelta.x;
        int screenSizeY = (int)effectUICanvas.sizeDelta.y;
        int defaultSideLength = (int)cameraChangeEffectUI.sizeDelta.x;

        int sideLength = screenSizeX > screenSizeY ? screenSizeX : screenSizeY;

        bool isChange = sideLength > defaultSideLength ? true : false;

        Vector2 screenSize = isChange ? new Vector2(sideLength, sideLength) : new Vector2(defaultSideLength, defaultSideLength);

        cameraChangeEffectUI.sizeDelta = screenSize;
    }
}
