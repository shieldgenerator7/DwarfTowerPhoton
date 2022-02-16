using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(RuleProcessor))]
public class RuleProcessorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.enabled = !EditorApplication.isPlaying;
        //Component Context
        bool anyNeedsCC = targets.Any(t => !hasCC(t as RuleProcessor));
        if (anyNeedsCC)
        {
            if (GUILayout.Button("Add ComponentContext (Editor only)"))
            {
                foreach (object target in targets)
                {
                    addCC(target as RuleProcessor);
                }
            }
        }
    }

    public static bool hasCC(RuleProcessor ruleProcessor)
    {
        return ruleProcessor.gameObject.FindComponent<ComponentContext>();
    }

    public static void addCC(RuleProcessor ruleProcessor)
    {
        if (!hasCC(ruleProcessor))
        {
            GameObject go = ruleProcessor.gameObject;
            ComponentContext cc = go.AddComponent<ComponentContext>();
            ComponentContextEditor.AutoSetup(cc);
            EditorUtility.SetDirty(ruleProcessor);
            EditorUtility.SetDirty(cc);
            EditorUtility.SetDirty(go);
        }
    }
}
