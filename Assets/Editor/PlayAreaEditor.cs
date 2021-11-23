using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayArea))]
public class PlayAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlayArea pa = (PlayArea)target;
        float prevWidth = pa.width;
        float prevHeight = pa.height;
        GUIContent lblWidth = new GUIContent("Width");
        lblWidth.tooltip = "The width of the play area";
        pa.width = EditorGUILayout.Slider(lblWidth, pa.width, 10, 100);
        GUIContent lblHeight = new GUIContent("Height");
        lblHeight.tooltip = "The height of the play area";
        pa.height = EditorGUILayout.Slider(lblHeight, pa.height, 10, 100);
        if (prevWidth != pa.width || prevHeight != pa.height)
        {
            pa.adjustPlayArea();
        }

        DrawDefaultInspector();
    }
}
