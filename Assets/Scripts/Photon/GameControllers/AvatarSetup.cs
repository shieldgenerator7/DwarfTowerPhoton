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
                PlayerInfo.instance.SelectedIndex,
                PlayerInfo.instance.getUniqueColorIndex()
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
        shadowSR.color = shadowSR.color.setRGB(
            teamToken.teamCaptain.teamColor
            );
        if (PV.IsMine)
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
    void RPC_AddCharacter(int characterIndex, int colorIndex)
    {
        characterValue = characterIndex;
        //Character
        myCharacter = Instantiate(
            PlayerInfo.instance.allCharacters[characterIndex].prefab,
            transform.position,
            transform.rotation,
            transform
            );
        //Color
        myCharacter.FindComponent<PlayerController>().playerColor =
            PlayerInfo.instance.allColors[colorIndex];
        //Stunned Delegate
        myCharacter.GetComponent<Stunnable>().onStunned +=
            (stunned) => GetComponent<BlinkEffect>().Blinking = stunned;
        GetComponent<BlinkEffect>().Start();
    }

    #region RPC Forwarding Methods
    //Moved here because the AvatarSetup is on the same object as the PhotonView,
    //but the Stunnable component is on a child of the object
    [PunRPC]
    void RPC_Stun()
    {
        GetComponentInChildren<Stunnable>().stun();
    }

    [PunRPC]
    void RPC_UpdateStats(StatLayer selfStats)
    {
        GetComponentInChildren<StatKeeper>().selfStats.Stats = selfStats;
    }
    #endregion
}
