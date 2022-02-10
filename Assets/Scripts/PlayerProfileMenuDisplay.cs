using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerProfileMenuDisplay : MenuDisplay
{
    public TMP_InputField txtPlayerName;

    void Start()
    {
        PlayerInfo playerInfo = PlayerInfo.instance;
        txtPlayerName.text = PlayerInfo.instance.playerName;
        txtPlayerName.onValueChanged.AddListener(
            (playerName) => playerInfo.playerName = playerName
            );
    }

}
