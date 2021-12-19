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
        asc.characterIndex = GUILayout.SelectionGrid(
            asc.characterIndex,
            playerInfo.allCharacters.ConvertAll(chr => chr.characterName).ToArray(),
            5
            );
        asc.colorIndex = GUILayout.SelectionGrid(
            asc.colorIndex,
            playerInfo.allColors.ConvertAll(
                color => ColorUtility.ToHtmlStringRGB(color)
                ).ToArray(),
            6
            );
    }
}