using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionUtils
{
    public static readonly float CropResolutionThreshold = 9 / 16f;

    private const int CanvasReferenceWidth = 1280;
    private const int CanvasReferenceHeight = 2560;

    public static float GetCanvasHeightDifferenceFromReference()
    {
        float originalScreenResolution = (float)Screen.width / Screen.height;
        float currentResolution = originalScreenResolution;

        if (originalScreenResolution > CropResolutionThreshold)
        {
            currentResolution = CropResolutionThreshold;
        }

        float currentCanvasHeight = CanvasReferenceWidth / currentResolution;
        return CanvasReferenceHeight - currentCanvasHeight;
    }
}
