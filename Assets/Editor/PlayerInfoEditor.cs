using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerInfo))]
public class PlayerInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.enabled = !EditorApplication.isPlaying;
        if (GUILayout.Button("Check"))
        {
            checkPlayerInfo(target as PlayerInfo);
        }
    }

    void checkPlayerInfo(PlayerInfo info)
    {
        //Sort character list
        info.characterSelection.itemList = info.characterSelection.itemList
            .OrderBy(charInfo => charInfo.characterName).ToList();
        //Ensure color list has all character default colors
        foreach (CharacterInfo charInfo in info.characterSelection.itemList)
        {
            if (!info.warmColorSelection.Contains(charInfo.defaultColor)
                && !info.coolColorSelection.Contains(charInfo.defaultColor)
                )
            {
                info.warmColorSelection.itemList.Add(charInfo.defaultColor);
                Debug.LogWarning(
                    "defaultColor not in the list! adding color: "
                    + ColorUtility.ToHtmlStringRGB(charInfo.defaultColor)
                    );
            }
        }
        //Sort color list
        info.warmColorSelection.itemList = info.warmColorSelection.itemList
            .OrderBy(color =>
            {
                float H, S, V;
                Color.RGBToHSV(color, out H, out S, out V);
                return H;
            }).ToList();
        info.coolColorSelection.itemList = info.coolColorSelection.itemList
            .OrderBy(color =>
            {
                float H, S, V;
                Color.RGBToHSV(color, out H, out S, out V);
                return H;
            }).ToList();
        //Set dirty
        EditorUtility.SetDirty(info);
    }
}
