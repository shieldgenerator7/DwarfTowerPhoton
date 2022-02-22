using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSetup : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=QbSI3Ajscgc

    private PhotonView PV;
    public int characterValue;
    public GameObject myCharacter;
    public SpriteRenderer shadowSR;

    private TeamToken teamToken;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            PV.RPC(
                "RPC_AddCharacter",
                RpcTarget.AllBuffered,
                PlayerInfo.instance.characterSelection.Index
                );
            Camera.main.GetComponent<CameraController>().FocusObject = gameObject;
        }
        teamToken = TeamToken.getTeamToken(gameObject);
        if (teamToken.teamCaptain)
        {
            setTeamIndicator();
        }
    }

    public void setTeamIndicator()
    {
        if (!teamToken)
        {
            teamToken = TeamToken.getTeamToken(gameObject);
        }
        //Team Color
        shadowSR.color = shadowSR.color.setRGB(
            teamToken.teamCaptain.teamColor
            );
        //RPC
        if (PV && PV.IsMine || this.isPhotonViewMine())
        {
            PV.RPC("RPC_SetTeamIndicator", RpcTarget.OthersBuffered);
        }
    }

    [PunRPC]
    void RPC_SetTeamIndicator()
    {
        setTeamIndicator();
    }

    [PunRPC]
    void RPC_AddCharacter(int characterIndex)
    {
        characterValue = characterIndex;
        //Character
        myCharacter = Instantiate(
            PlayerInfo.instance.characterSelection[characterIndex].prefab,
            transform.position,
            transform.rotation,
            transform
            );
        myCharacter.FindComponent<ComponentContext>().InitializeComponents();
        //Stunned Delegate
        myCharacter.GetComponent<StatusKeeper>().onStatusChanged +=
            (status) => GetComponent<BlinkEffect>().Blinking = status.Has(StatusEffect.STUNNED);
        GetComponent<BlinkEffect>().Start();
    }

    public void updatePlayerInfo(int colorIndex, string playerName)
    {
        PV.RPC("RPC_UpdatePlayerInfo", RpcTarget.AllBuffered, colorIndex, playerName);
    }

    [PunRPC]
    private void RPC_UpdatePlayerInfo(int colorIndex, string playerName)
    {
        PlayerController playerController = myCharacter.FindComponent<PlayerController>();
        //Player Color
        int colorGroupIndex = teamToken.teamCaptain.colorGroupIndex;
        playerController.playerColor = PlayerInfo.instance
             .colorGroups[colorGroupIndex][colorIndex];
        //Player Name
        playerController.PlayerName = playerName;
    }

    #region RPC Forwarding Methods

    [PunRPC]
    void RPC_UpdateStats(StatLayer selfStats)
    {
        GetComponentInChildren<StatKeeper>().selfStats.Stats = selfStats;
    }
    #endregion
}
