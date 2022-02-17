using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanController : PlayerController, IPunObservable
{
    [Header("Settings")]

    [Tooltip("How much roll he gains/loses when healed/damaged")]
    public float hpRollChange = 1;
    [Tooltip("How much roll he discharges with huge shot")]
    public float dischargeRollChange = 2;

    private StatLayer minRollLayer;
    public StatLayer maxRollLayer;

    public StatLayer rollingLayer;

    //TODO: Re-implement snowman damaging players when he rolls into them
    //public List<EntityType> standDamageTypes;
    //public List<EntityType> rollingDamageTypes;

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
        HealthPool healthPool = context.healthPool;
        StatKeeper statKeeper = context.statKeeper;
        PhotonView PV = context.PV;
        //Roll
        rollAbility.onRollChanged += UpdateStats;
        rollAbility.onRollingChanged += (rolling) =>
        {
            UpdateSprite(rolling);
            if (rolling)
            {
                maxRollLayer.damage = maxRollLayer.maxHits;
                statKeeper.selfStats.addLayer(rollAbility.abilityID, rollingLayer);
            }
            else
            {
                maxRollLayer.damage = -1;
                statKeeper.selfStats.removeLayer(rollAbility.abilityID);
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
        //PhotonView Observable
        PV.ObservedComponents.Add(this);
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
        HealthPool healthPool = context.healthPool;
        StatKeeper statKeeper = context.statKeeper;
        PhotonView PV = context.PV;
        healthPool.onDamaged -= OnDamage;
        //
        curLayer = StatLayer.Lerp(minRollLayer, maxRollLayer, percentage);
        statKeeper.selfStats.addLayer(PV.ViewID, curLayer);
        //
        healthPool.onDamaged += OnDamage;
    }

    private void UpdateSprite(bool rolling)
    {
        SpriteRenderer sr = context.sr;
        sr.sprite = (rolling) ? rollSprite : standSprite;
    }

    protected override void onAminaEmpty(float amina)
    {
        base.onAminaEmpty(amina);
        rollAbility.OnButtonCanceled();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //2021-12-18: copied from https://doc.photonengine.com/en-us/pun/current/demos-and-tutorials/pun-basics-tutorial/player-networking
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(rollAbility.Rolling);
        }
        else
        {
            // Network player, receive data
            UpdateSprite((bool)stream.ReceiveNext());
        }
    }
}
