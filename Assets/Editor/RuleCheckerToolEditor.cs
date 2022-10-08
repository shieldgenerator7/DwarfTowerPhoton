using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RuleCheckerTool))]
public class RuleCheckerToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RuleCheckerTool rct = (target as RuleCheckerTool);

        if (GUILayout.Button("Select Folder"))
        {
            Selection.selectionChanged -= OnSelectFolder;
            Selection.selectionChanged += OnSelectFolder;
        }
        if (GUILayout.Button("Clear Folder"))
        {
            Selection.selectionChanged -= OnSelectFolder;
            rct.folderToSearch = "";
        }

        DrawDefaultInspector();

        if (GUILayout.Button("Find Rules"))
        {
            rct.findRules();
        }
        if (GUILayout.Button("Check for Errors"))
        {
            rct.checkRules();
        }
        GUILayout.Label("Errors:");
        rct.ErrorList.ForEach(error => GUILayout.Label(error));
    }

    private void OnSelectFolder()
    {
        RuleCheckerTool rct = (target as RuleCheckerTool);
        string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        rct.folderToSearch = path;
        //
        Selection.selectionChanged -= OnSelectFolder;
        //Selection.SetActiveObjectWithContext(rct, rct.gameObject);
        //Selection.activeGameObject = rct.gameObject;
    }
}
