using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSetup : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=QbSI3Ajscgc

    private PhotonView PV;
    public int characterValue;
    public GameObject myCharacter;

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
                PlayerInfo.instance.ColorIndex
                );
            Camera.main.GetComponent<CameraController>().FocusObject = gameObject;
        }
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
    }

    //Moved here because the AvatarSetup is on the same object as the PhotonView,
    //but the Stunnable component is on a child of the object
    [PunRPC]
    void RPC_Stun()
    {
        GetComponentInChildren<Stunnable>().stun();
    }
}
