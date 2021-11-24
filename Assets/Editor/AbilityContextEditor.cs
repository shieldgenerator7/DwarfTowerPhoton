using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbilityContext))]
public class AbilityContextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.enabled = !EditorApplication.isPlaying;
        if (GUILayout.Button("Auto set abilityIDs (Editor Only)"))
        {
            AbilityContext ac = (AbilityContext)target;
            int nextID = 0;

            List<PlayerAbility> abilities = new List<PlayerAbility>();
            abilities.AddRange(ac.GetComponents<PlayerAbility>());
            abilities.AddRange(ac.GetComponentsInChildren<PlayerAbility>());

            foreach (PlayerAbility pa in abilities)
            {
                pa.abilityID = nextID;
                nextID++;
                EditorUtility.SetDirty(pa);
            }
        }
    }
}
