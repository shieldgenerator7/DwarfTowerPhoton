using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rule))]
public class RuleEditor : Editor
{
    static bool triggerFoldout = false;
    static bool conditionFoldout = false;
    static bool actionFoldout = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Triggers
        triggerFoldout = EditorGUILayout.Foldout(triggerFoldout, "Triggers", true);
        if (triggerFoldout)
        {
            //OnDieTrigger
            if (GUILayout.Button("OnDieTrigger"))
            {
                (target as Rule).triggers.Add(new OnDieTrigger());
                (target as Rule).otestdsf = new OnDieTrigger();
            }
        }
        //Conditions
        conditionFoldout = EditorGUILayout.Foldout(conditionFoldout, "Conditions", true);
        if (conditionFoldout)
        {
            GUILayout.Label("No conditions to show");
        }
        //Actions
        actionFoldout = EditorGUILayout.Foldout(actionFoldout, "Actions", true);
        if (actionFoldout)
        {
            GUILayout.Label("No actions to show");
        }
    }
}
