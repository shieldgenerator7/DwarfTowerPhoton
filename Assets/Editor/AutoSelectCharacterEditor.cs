using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutoSelectCharacter))]
public class AutoSelectCharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        PlayerInfo playerInfo = FindObjectOfType<PlayerInfo>();
        AutoSelectCharacter asc = target as AutoSelectCharacter;
        int prevCharIndex = asc.characterIndex;
        int prevWarmColorIndex = asc.warmColorIndex;
        int prevCoolColorIndex = asc.coolColorIndex;
        asc.characterIndex = GUILayout.SelectionGrid(
            asc.characterIndex,
            playerInfo.characterSelection.itemList.ConvertAll(chr => chr.characterName).ToArray(),
            5
            );
        asc.warmColorIndex = GUILayout.SelectionGrid(
            asc.warmColorIndex,
            playerInfo.warmColorSelection.itemList.ConvertAll(
                color => ColorUtility.ToHtmlStringRGB(color)
                ).ToArray(),
            6
            );
        asc.coolColorIndex = GUILayout.SelectionGrid(
            asc.coolColorIndex,
            playerInfo.coolColorSelection.itemList.ConvertAll(
                color => ColorUtility.ToHtmlStringRGB(color)
                ).ToArray(),
            6
            );
        if (prevCharIndex != asc.characterIndex
            || prevWarmColorIndex != asc.warmColorIndex
            || prevCoolColorIndex != asc.coolColorIndex
            )
        {
            EditorUtility.SetDirty(asc);
        }
    }
}
