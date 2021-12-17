using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanController : PlayerController
{
    [Header("Settings")]

    [Tooltip("How much roll he gains/loses when healed/damaged")]
    public float hpRollChange = 1;

    private StatLayer minRollLayer;
    public StatLayer maxRollLayer;

    public StatLayer rollingLayer;

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
        curLayer = StatLayer.Lerp(minRollLayer, maxRollLayer, percentage);
        statKeeper.selfStats.addLayer(PV.ViewID, curLayer);
    }

    protected override void onAminaEmpty(float amina)
    {
        base.onAminaEmpty(amina);
        rollAbility.OnButtonCanceled();
    }
}
