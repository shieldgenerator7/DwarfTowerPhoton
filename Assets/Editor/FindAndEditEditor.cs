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

        FindAndEdit findAndEdit = (target as FindAndEdit);

        GUI.enabled = findAndEdit.findComponent;
        if (GUILayout.Button("Find"))
        {
            findAndEdit.FindMonoBehaviours();
        }

        GUI.enabled = findAndEdit.foundComponents.Count > 0;
        if (GUILayout.Button("Select"))
        {
            findAndEdit.SelectComponents();
        }

        GUI.enabled = findAndEdit.foundComponents.Count > 0;
        if (GUILayout.Button("Select GameObjects"))
        {
            findAndEdit.SelectGameObjects();
        }

        GUI.enabled = findAndEdit.addComponent;
        if (GUILayout.Button("Add"))
        {
            findAndEdit.AddComponent();
        }

        GUI.enabled = findAndEdit.removeComponent;
        if (GUILayout.Button("Remove"))
        {
            findAndEdit.RemoveComponent();
        }

        GUI.enabled = findAndEdit.findComponent;
        if (GUILayout.Button("Clear Settings"))
        {
            findAndEdit.ClearSettings();
        }
    }
}
