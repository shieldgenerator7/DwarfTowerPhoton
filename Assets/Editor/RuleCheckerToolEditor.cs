using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RuleCheckerTool))]
public class RuleCheckerToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RuleCheckerTool rct = (target as RuleCheckerTool);

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
}
