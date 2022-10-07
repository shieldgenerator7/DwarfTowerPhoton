using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RuleChecker))]
public class RuleCheckerEditor : Editor
{
    static bool testOn = false;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Test"))
        {
            RuleChecker rc = (target as RuleChecker);
            rc.testOn = !rc.testOn;
        }
    }
}
