using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(AbilityContext))]
public class AbilityContextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.enabled = !EditorApplication.isPlaying;
        if (GUILayout.Button("Auto set abilityIDs (Editor Only)"))
        {
            foreach (object target in targets)
            {
                autoSetAbilityIDs(target as AbilityContext);
            }
        }
        if (GUILayout.Button("Validify abilities (Editor Only)"))
        {
            foreach (object target in targets)
            {
                validify(target as AbilityContext);
            }
        }
    }

    public void autoSetAbilityIDs(AbilityContext abilityContext)
    {
        int nextID = 0;

        List<PlayerAbility> abilities = new List<PlayerAbility>();
        abilities.AddRange(abilityContext.GetComponents<PlayerAbility>());
        abilities.AddRange(abilityContext.GetComponentsInChildren<PlayerAbility>());

        foreach (PlayerAbility pa in abilities)
        {
            pa.abilityID = nextID;
            nextID++;
            EditorUtility.SetDirty(pa);
        }
    }

    public void validify(AbilityContext abilityContext)
    {
        abilityContext.abilities.RemoveAll(ablty => !ablty);
        EditorUtility.SetDirty(abilityContext);
        EditorUtility.SetDirty(abilityContext.gameObject);
    }
}
