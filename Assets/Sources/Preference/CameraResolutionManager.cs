using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolutionManager : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private bool needLetterBox;

    void Start()
    {
        float originalScreenResolution = (float)Screen.width / Screen.height;

        if (originalScreenResolution > ResolutionUtils.CropResolutionThreshold)
        {
            Rect rect = gameCamera.rect;
            float scaleheight = originalScreenResolution / ResolutionUtils.CropResolutionThreshold;
            float scalewidth = 1f / scaleheight;

            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;

            gameCamera.rect = rect;
        }

        gameCamera.fieldOfView = Camera.HorizontalToVerticalFieldOfView(60f, gameCamera.aspect);
    }

    void OnPreCull()
    {
        if (needLetterBox)
        {
            GL.Clear(true, true, Color.black);
        }
    }
}
