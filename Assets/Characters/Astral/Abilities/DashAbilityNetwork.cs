using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbilityNetwork : MonoBehaviour
{
    public DashAbility dashAbility;

    [PunRPC]
    void RPC_OnDash(bool dashing)
    {
        dashAbility.callOnDashDelegate(dashing);
    }
}
