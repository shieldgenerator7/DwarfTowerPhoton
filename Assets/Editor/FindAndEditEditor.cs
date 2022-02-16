using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FindAndEdit))]
public class FindAndEditEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Find"))
        {
            (target as FindAndEdit).FindMonoBehaviours();
        }
        if (GUILayout.Button("Select"))
        {
            (target as FindAndEdit).SelectGameObjects();
        }
        if (GUILayout.Button("Add"))
        {
            (target as FindAndEdit).AddComponent();
        }
        if (GUILayout.Button("Remove"))
        {
            (target as FindAndEdit).RemoveComponent();
        }
        if (GUILayout.Button("Clear Settings"))
        {
            (target as FindAndEdit).ClearSettings();
        }
    }
}
