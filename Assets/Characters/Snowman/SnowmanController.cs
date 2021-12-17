using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanController : PlayerController
{
    [Header("Settings")]

    [Tooltip("How much roll he gains/loses when healed/damaged")]
    public float hpRollChange = 1;

    public StatLayer minLayer;
    public StatLayer maxLayer;

    [Header("Components")]

    public Sprite standSprite;
    public Sprite rollSprite;

    public RollAbility rollAbility;

    private StatLayer curLayer;

    protected override void Start()
    {
        base.Start();
        if (PV.IsMine)
        {
            //Register delegates
            rollAbility.onRollChanged += UpdateStats;
            rollAbility.onRollingChanged += (rolling) =>
            {
                sr.sprite = (rolling) ? rollSprite : standSprite;
            };
            healthPool.onDamaged += (hp) =>
            {
                rollAbility.RollAmount -= hpRollChange;
            };
            healthPool.onDied += (hp) =>
            {
                rollAbility.RollAmount = 0;
            };
            //Init values
            UpdateStats(rollAbility.RollAmount);
        }
    }

    private void UpdateStats(float percentage)
    {
        curLayer = StatLayer.LerpAdd(minLayer, maxLayer, percentage);
        statKeeper.selfStats.addLayerAdd(PV.ViewID, curLayer);
    }
}
