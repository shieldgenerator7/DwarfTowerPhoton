using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanController : PlayerController
{
    public StatLayer minLayer;
    public StatLayer maxLayer;

    [System.Serializable]
    public struct KeyFrame
    {
        public float health;
        public StatLayer stats;
    }

    private float percentage = 0;
    private StatLayer curLayer;

    protected override void Start()
    {
        base.Start();
        if (PV.IsMine)
        {
            //Register delegates
            healthPool.onChanged += (hp) =>
            {
                float percent = hp / healthPool.MaxHealth;
                if (percent != this.percentage)
                {
                    UpdateStats(percent);
                }
            };
            //Init values
            UpdateStats(healthPool.Health);
        }
    }

    private void UpdateStats(float percentage)
    {
        this.percentage = percentage;
        curLayer = StatLayer.Lerp(minLayer, maxLayer, percentage);
        statKeeper.selfStats.addLayerAdd(PV.ViewID, curLayer);
    }
}
