using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedShotController : ShotController
{
    public StatMatrixComponent multiplierLayer;

    protected float multiplier = 0;

    public Sprite previewSprite;//used for constructs that can be manually destroyed / upgraded

    [Tooltip("If manually deconstructed, how much amina is refunded")]
    public float aminaRefund = 20;

    protected override void Start()
    {
        base.Start();
    }

    public void chargeStats(float multiplier)
    {
        if (PV == null)
        {
            Start();
        }
        this.multiplier = multiplier;
        PV.RPC("RPC_ChargeStats", RpcTarget.All, this.multiplier);
    }

    [PunRPC]
    public void RPC_ChargeStats(float multiplier)
    {
        StatLayer stats = statKeeper.selfStats.StatBase.Charge(multiplier, multiplierLayer.Stats);
    }

    /// <summary>
    /// Upgrades stats after the object has already been initially charged
    /// </summary>
    /// <param name="newMulitplier"></param>
    public void upgradeStats(float newMulitplier)
    {
        float totalMulitplier = newMulitplier + this.multiplier;
        float converterMultiplier = newMulitplier / this.multiplier;
        chargeStats(converterMultiplier);
        this.multiplier = totalMulitplier;
    }
}
