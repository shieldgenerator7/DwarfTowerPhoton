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

    public List<EntityType> standDamageTypes;
    public List<EntityType> rollingDamageTypes;

    [Header("Components")]

    public Sprite standSprite;
    public Sprite rollSprite;

    public GunController hugeShotAbility;
    public RollAbility rollAbility;

    private StatLayer curLayer;

    protected override void InitializeSettings()
    {
        base.InitializeSettings();
        //Init layers
        minRollLayer = new StatLayer(1);
    }

    protected override void RegisterDelegates()
    {
        base.RegisterDelegates();
        //Roll
        rollAbility.onRollChanged += UpdateStats;
        rollAbility.onRollingChanged += (rolling) =>
        {
            sr.sprite = (rolling) ? rollSprite : standSprite;
            if (rolling)
            {
                maxRollLayer.damage = maxRollLayer.maxHits;
                statKeeper.selfStats.addLayer(rollAbility.abilityID, rollingLayer);
                damager.damagableTypes = rollingDamageTypes;
            }
            else
            {
                maxRollLayer.damage = -1;
                statKeeper.selfStats.removeLayer(rollAbility.abilityID);
                damager.damagableTypes = standDamageTypes;
            }
        };
        //Health
        healthPool.onDamaged += OnDamage;
        healthPool.onDied += (hp) =>
        {
            rollAbility.RollAmount = 0;
        };
        //Huge shot
        hugeShotAbility.onShotFired += (shot, targetPos, targetDir) =>
        {
            rollAbility.RollAmount -= dischargeRollChange;
        };
    }

    protected override void InvokeDelegates()
    {
        base.InvokeDelegates();
        //Init values
        UpdateStats(rollAbility.RollAmount);
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
