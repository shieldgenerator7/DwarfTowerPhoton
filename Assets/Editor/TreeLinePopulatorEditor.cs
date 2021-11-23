using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TreeLinePopulator))]
public class TreeLinePopulatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TreeLinePopulator tlp = ((TreeLinePopulator)target);
        float prevPercent = tlp.newPercent;

        DrawDefaultInspector();

        bool inEditor = !EditorApplication.isPlaying;
        GUI.enabled = inEditor;
        if (GUILayout.Button("Add percentage (Editor Only)"))
        {
            tlp.percentages.Add(tlp.newPercent);
            tlp.populate();
        }
        if (inEditor)
        {
            if (prevPercent != tlp.newPercent)
            {
                tlp.percentages[tlp.percentages.Count - 1] = tlp.newPercent;
                tlp.adjustLastTrees();
            }
        }
        if (GUILayout.Button("Populate Tree Line (Editor Only)"))
        {
            tlp.populate();
        }
    }
}
