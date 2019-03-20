using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedShotController : ShotController
{
    //
    // Multipliers
    // How much the multiplier passed into chargeStats() effects that stat
    // 0   = no multiplier received
    // 0.5 = half multiplier received
    // 1   = exact multiplier received
    // 2   = double multiplier received
    public float speedChargeMultiplier = 1;
    public float initialDamageChargeMultiplier = 1;
    public float damagePerSecondChargeMultiplier = 1;
    public float stunDurationChargeMultiplier = 1;
    public float knockbackDistanceChargeMultiplier = 1;
    public float maxHealthChargeMultiplier = 1;

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
        PV.RPC("RPC_ChargeStats", RpcTarget.All, multiplier);
    }

    [PunRPC]
    public void RPC_ChargeStats(float multiplier) { 
        speed = chargeStat(speed, multiplier, speedChargeMultiplier);
        initialDamage = chargeStat(initialDamage, multiplier, initialDamageChargeMultiplier);
        damagePerSecond = chargeStat(damagePerSecond, multiplier, damagePerSecondChargeMultiplier);
        stunDuration = chargeStat(stunDuration, multiplier, stunDurationChargeMultiplier);
        knockbackDistance = chargeStat(knockbackDistance, multiplier, knockbackDistanceChargeMultiplier);
        maxHealth = chargeStat(maxHealth, multiplier, maxHealthChargeMultiplier);
    }

    /// <summary>
    /// Returns the charged value of the given stat with the given multiplier and given stat charge multiplier
    /// Examples:
    /// 2, 1, 1 -> 2
    /// 2, 2, 1 -> 4
    /// 2, 0.5, 1 -> 1
    /// 2, 1, 2 -> 2
    /// 2, 2, 2 -> 6
    /// 2, 0.5, 2 -> 0
    /// 2, 1, 0.5 -> 2
    /// 2, 2, 0.5 -> 3
    /// 2, 0.5, 0.5 -> 1.5
    /// </summary>
    /// <param name="multiplier"></param>
    /// <param name="stat"></param>
    /// <param name="statChargeMultiplier"></param>
    private float chargeStat(float stat, float multiplier, float statChargeMultiplier)
    {
        float newStat = stat * multiplier;
        float diff = (newStat - stat);
        float keptDiff = diff * statChargeMultiplier;
        return Mathf.Max(0, stat + keptDiff);
    }
}
