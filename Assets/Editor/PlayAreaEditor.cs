using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayArea))]
public class PlayAreaEditor : Editor
{
    static string warningLabel = "Width and Height are for test purposes only, not used in game! " +
            "\nLook at MapProfile.playAreaSize on a scriptableobject asset.";

    public override void OnInspectorGUI()
    {
        PlayArea pa = (PlayArea)target;
        float prevWidth = pa.width;
        float prevHeight = pa.height;

        GUILayout.Box(warningLabel);
        DrawDefaultInspector();

        if (prevWidth != pa.width || prevHeight != pa.height)
        {
            pa.adjustPlayArea(pa.width, pa.height);
        }

    }
}
