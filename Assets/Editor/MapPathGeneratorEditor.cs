using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapPathGenerator))]
public class MapPathGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.enabled = EditorApplication.isPlaying;
        if (GUILayout.Button("Generate (Play mode only)"))
        {
            (target as MapPathGenerator).generateMapPath();
        }
    }
}
