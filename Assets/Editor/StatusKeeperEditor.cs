using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(StatusKeeper))]
public class StatusKeeperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Set all allowed"))
        {
            List<StatusEffect> effects = new List<StatusEffect>();
            foreach (StatusEffect effect in Enum.GetValues(typeof(StatusEffect)))
            {
                effects.Add(effect);
            }
            foreach (object trgt in targets)
            {
                SetAllAllowed((StatusKeeper)trgt, effects);
            }
        }
    }

    private void SetAllAllowed(StatusKeeper statusKeeper, List<StatusEffect> effects)
    {
        statusKeeper.AllowedStatus = new StatusLayer(effects);
        EditorUtility.SetDirty(statusKeeper);
        EditorUtility.SetDirty(statusKeeper.gameObject);
    }
}
