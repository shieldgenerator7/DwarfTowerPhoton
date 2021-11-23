using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayArea))]
public class PlayAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PlayArea pa = (PlayArea)target;
        float prevWidth = pa.width;
        float prevHeight = pa.height;

        DrawDefaultInspector();

        if (prevWidth != pa.width || prevHeight != pa.height)
        {
            pa.adjustPlayArea();
        }

    }
}
