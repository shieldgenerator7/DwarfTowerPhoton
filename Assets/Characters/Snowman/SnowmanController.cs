using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanController : PlayerController
{
    [Header("Settings")]

    [Tooltip("How much roll he gains/loses when healed/damaged")]
    public float hpRollChange = 1;
    [Tooltip("How much roll he discharges with huge shot")]
    public float dischargeRollChange = 2;

    private StatLayer minRollLayer;
    public StatLayer maxRollLayer;

    public StatLayer rollingLayer;

    [Header("Components")]

    public Sprite standSprite;
    public Sprite rollSprite;

    public GunController hugeShotAbility;
    public RollAbility rollAbility;

    private StatLayer curLayer;

    protected override void Start()
    {
        base.Start();
        if (PV.IsMine)
        {
            //Init layers
            minRollLayer = new StatLayer(1);
            //Register delegates
            rollAbility.onRollChanged += UpdateStats;
            rollAbility.onRollingChanged += (rolling) =>
            {
                sr.sprite = (rolling) ? rollSprite : standSprite;
                if (rolling)
                {
                    statKeeper.selfStats.addLayer(rollAbility.abilityID, rollingLayer);
                }
                else
                {
                    statKeeper.selfStats.removeLayer(rollAbility.abilityID);
                }
            };
            healthPool.onDamaged += OnDamage;
            healthPool.onDied += (hp) =>
            {
                rollAbility.RollAmount = 0;
            };
            hugeShotAbility.onShotFired += (shot, targetPos, targetDir) =>
            {
                rollAbility.RollAmount -= dischargeRollChange;
            };
            //Init values
            UpdateStats(rollAbility.RollAmount);
        }
    }

    private void OnDamage(float health)
    {
        rollAbility.RollAmount -= hpRollChange;
    }

    private void UpdateStats(float percentage)
    {
        healthPool.onDamaged -= OnDamage;
        //
        curLayer = StatLayer.Lerp(minRollLayer, maxRollLayer, percentage);
        statKeeper.selfStats.addLayer(PV.ViewID, curLayer);
        //
        healthPool.onDamaged += OnDamage;
    }

    protected override void onAminaEmpty(float amina)
    {
        base.onAminaEmpty(amina);
        rollAbility.OnButtonCanceled();
    }
}
