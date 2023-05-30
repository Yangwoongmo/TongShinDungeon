using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDecoration : MonoBehaviour
{
    public DecoType type;

    public enum DecoType
    {
        COLUMN,
        VINE1,
        VINE2,
        ASH_TRAY,
        BARREL,
        CLAY_POT1,
        CLAY_POT2,
        DEAD_BODY,
        HAIR,
        IRON_POT1,
        IRON_POT2,
        OFFERING_BOX,
        SKULL,
        STONE_GRAVE,
        TORCH,
        WOOD_SIGN,
        CLAY_POT1_S
    }
}
