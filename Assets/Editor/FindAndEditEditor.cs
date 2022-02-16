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
    }
}
