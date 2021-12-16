using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanController : PlayerController
{
    [Tooltip("How much roll he loses when damaged")]
    public float hitRollLoss = 1;

    public StatLayer minLayer;
    public StatLayer maxLayer;

    public RollAbility rollAbility;

    private StatLayer curLayer;

    protected override void Start()
    {
        base.Start();
        if (PV.IsMine)
        {
            //Register delegates
            rollAbility.onRollChanged += UpdateStats;
            healthPool.onDamaged += (hp) =>
            {
                rollAbility.RollAmount -= hitRollLoss;
            };
            //Init values
            UpdateStats(rollAbility.RollAmount);
        }
    }

    private void UpdateStats(float percentage)
    {
        curLayer = StatLayer.Lerp(minLayer, maxLayer, percentage);
        statKeeper.selfStats.addLayerAdd(PV.ViewID, curLayer);
    }
}
