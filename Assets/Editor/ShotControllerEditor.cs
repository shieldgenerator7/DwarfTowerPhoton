using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ShotController), editorForChildClasses: true)]
public class ShotControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.enabled = !EditorApplication.isPlaying;
        if (GUILayout.Button("Extract Damager (Editor only)"))
        {
            foreach(object target in targets)
            {
                extractDamager((ShotController)target);
            }
        }
    }

    void extractDamager(ShotController shot)
    {
        Damager damager = shot.gameObject.FindComponent<Damager>();
        if (!damager)
        {
            damager = shot.gameObject.AddComponent<Damager>();
        }
        damager.damage = shot.stats.damage;
        damager.damagableTypes = shot.damagableTypes;
        EditorUtility.SetDirty(damager);
    }
}
