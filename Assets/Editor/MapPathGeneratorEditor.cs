using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapPathGenerator))]
public class MapPathGeneratorEditor : Editor
{
    static bool autoRegenerate = false;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.enabled = EditorApplication.isPlaying;
        if (GUILayout.Button("Generate (Play mode only)"))
        {
            regenerate(target as MapPathGenerator);
        }
        autoRegenerate = GUILayout.Toggle(autoRegenerate, "Auto Regenerate");
        if (autoRegenerate)
        {
            regenerate(target as MapPathGenerator);
        }
    }

    void regenerate(MapPathGenerator mpg)
    {
        mpg.generateMapPath(
            FindObjectOfType<MapGenerator>().mapProfile
            );
    }
}
