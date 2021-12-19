using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatKeeper : MonoBehaviour
{
    public StatMatrix selfStats;

    public StatMatrix shotStats;

    public StatMatrix constructStats;

    private PhotonView PV;

    private void Start()
    {
        selfStats.init();
        shotStats.init();
        constructStats.init();
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
        shotStats.triggerEvent();
        constructStats.triggerEvent();
    }
}
