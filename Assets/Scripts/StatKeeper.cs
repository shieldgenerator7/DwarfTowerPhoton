using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatKeeper : MonoBehaviour
{
    public StatMatrix selfStats;

    private PhotonView PV;

    private void Start()
    {
        selfStats.init();
        //Register RPC events
        PV = gameObject.FindComponent<PhotonView>();
        if (PV.IsMine)
        {
            selfStats.onStatChanged +=
                (selfStats) => PV.RPC("RPC_UpdateStats", RpcTarget.Others, selfStats);
        }
    }

    public void triggerEvents()
    {
        selfStats.triggerEvent();
    }
}
