using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ComponentContext))]
public class ComponentContextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUI.enabled = !EditorApplication.isPlaying;
        //Auto setup
        if (GUILayout.Button("Auto setup (Editor only)"))
        {
            foreach(object target in targets)
            {
                AutoSetup(target as ComponentContext);
            }
        }
    }

    private void AutoSetup(ComponentContext context)
    {
        PlayerController pc = context.gameObject.FindComponent<PlayerController>();
        pc.context = context;
        EditorUtility.SetDirty(context);
        EditorUtility.SetDirty(pc);
        EditorUtility.SetDirty(context.gameObject);
    }
}
