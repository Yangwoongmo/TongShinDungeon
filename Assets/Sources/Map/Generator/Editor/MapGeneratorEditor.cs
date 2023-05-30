using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator generator = (MapGenerator)target;
        if (GUILayout.Button("Generate map"))
        {
            generator.generateMap();
        }

        if(GUILayout.Button("Save current wall/floor material"))
        {
            generator.SaveCurrentWallFloorMaterialState();
        }

        if(GUILayout.Button("Restore wall/floor material"))
        {
            generator.RestoreWallFloorMaterialState();
        }

        if (GUILayout.Button("Save current decorations"))
        {
            generator.SaveCurrentMapDecorationState();
        }

        if (GUILayout.Button("Restore decorations"))
        {
            generator.RestoreMapDecoration();
        }
    }
}
