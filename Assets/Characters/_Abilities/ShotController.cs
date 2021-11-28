using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    [Tooltip("Should this shot destroy itself when it hits an undestroyable object (ex: border walls, caravan)?")]
    public bool destroyOnIndestructible = true;

    //
    // Components
    //
    protected StatKeeper statKeeper { get; private set; }

    protected HealthPool health { get; private set; }
    
    protected Rigidbody2D rb2d { get; private set; }

    protected PhotonView photonView;
    public PhotonView PV
    {
        get
        {
            if (photonView == null)
            {
                photonView = GetComponent<PhotonView>();
            }
            return photonView;
        }
    }

    private PlayerController _owner;
    protected virtual PlayerController owner
    {
        get => _owner;
        set => _owner = value;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //RB2D
        rb2d = gameObject.FindComponent<Rigidbody2D>(); 
        if (rb2d)
        {
            rb2d.velocity = transform.up;
        }
        //HealthPool
        health = GetComponent<HealthPool>();
        health.onDied += () =>
        {
            if (PV.IsMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
            else
            {
                GetComponent<Collider2D>().enabled = false;
            }
        };
        //StatKeeper
        statKeeper = gameObject.FindComponent<StatKeeper>();
        statKeeper.selfStats.onStatChanged += onStatsChanged;
        onStatsChanged(statKeeper.selfStats.Stats);
        health.Health = health.MaxHealth;
    }

    protected virtual void onStatsChanged(StatLayer stats)
    {
        if (rb2d)
        {
            rb2d.velocity = rb2d.velocity.normalized * stats.moveSpeed;
        }
        health.MaxHealth = stats.maxHits;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        processCollision(collision, true);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        processCollision(collision, false);
    }

    protected virtual void processCollision(Collider2D collision, bool useInitialDamage)
    {
        //If this shot should destroy itself on indestructibles,
        if (destroyOnIndestructible)
        {
            if (TeamToken.onSameTeam(gameObject, collision.gameObject))
            {
                //don't get destroyed on teammates objects
                return;
            }
            //If object is indestructible and solid,
            HealthPool hp = collision.gameObject.GetComponent<HealthPool>();
            if (!hp && !collision.isTrigger)
            {
                //Destroy this shot
                health.Health = 0;
            }
        }
    }

    [PunRPC]
    protected void RPC_SelfDestruct()
    {
        health.Health = 0;
    }

    public void switchOwner(PlayerController pc)
    {
        int ownerID = -1;
        if (pc)
        {
            PV.TransferOwnership(pc.PV.Owner);
            ownerID = pc.PV.ViewID;
        }
        PV.RPC("RPC_SwitchOwner", RpcTarget.AllBuffered, ownerID);
    }

    [PunRPC]
    protected void RPC_SwitchOwner(int ownerID)
    {
        owner = null;
        if (ownerID >= 0)
        {
            foreach (PlayerController pc in FindObjectsOfType<PlayerController>())
            {
                if (pc.PV.ViewID == ownerID)
                {
                    owner = pc;
                }
            }
        }
    }
}
