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
        info.allCharacters = info.allCharacters
            .OrderBy(charInfo => charInfo.characterName).ToList();
        //Ensure color list has all character default colors
        foreach (CharacterInfo charInfo in info.allCharacters)
        {
            if (!info.allColors.Contains(charInfo.defaultColor))
            {
                info.allColors.Add(charInfo.defaultColor);
                Debug.LogWarning(
                    "defaultColor not in the list! adding color: "
                    + ColorUtility.ToHtmlStringRGB(charInfo.defaultColor)
                    );
            }
        }
        //Sort color list
        info.allColors = info.allColors
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
