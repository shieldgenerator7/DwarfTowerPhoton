using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RuleChecker))]
public class RuleCheckerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RuleChecker rc = (target as RuleChecker);

        if (GUILayout.Button("Test"))
        {
            rc.checkRule();
        }
        GUILayout.Label("Errors:");
        rc.ErrorList.ForEach(error => GUILayout.Label(error));
    }
}
