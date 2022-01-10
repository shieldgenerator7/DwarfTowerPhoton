using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=QbSI3Ajscgc
    
    public void OnClickCharacterPick(int characterIndex)
    {
        Debug.Log($"Character Picked: {characterIndex}");
        if (PlayerInfo.instance != null)
        {
            PlayerInfo.instance.characterSelection.Index = characterIndex;
            //PlayerPrefs.SetInt("MyCharacter", characterIndex);
        }
    }
}
