﻿using Photon.Pun;
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
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.instance.mySelectedCharacter);
            Camera.main.GetComponent<CameraController>().FocusObject = gameObject;
        }
    }

    [PunRPC]
    void RPC_AddCharacter(int characterIndex)
    {
        characterValue = characterIndex;
        myCharacter = Instantiate(
            PlayerInfo.instance.allCharacters[characterIndex],
            transform.position,
            transform.rotation,
            transform
            );
    }
}
