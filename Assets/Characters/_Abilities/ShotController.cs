using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    //
    // Settings
    //
    [SerializeField]
    protected StatLayer statBase = new StatLayer();
    /// <summary>
    /// Current stats
    /// </summary>
    public StatLayer stats
    {
        get => _stats;
        protected set
        {
            _stats = value;
        }
    }
    [SerializeField]
    private StatLayer _stats;

    [Tooltip("The EntityTypes that this shot can damage")]
    private List<EntityType> damagableTypes;

    //TODO: Remove these checkboxes when Unity fixes its editor list bug
    public bool damagesShots = false;
    public bool damagesObjects = false;
    public bool damagesPlayers = false;

    [Tooltip("Should this shot destroy itself when it hits an undestroyable object (ex: border walls, caravan)?")]
    public bool destroyOnIndestructible = true;

    //
    // Components
    //

    protected HealthPool health { get; private set; }


    private new Rigidbody2D rigidbody2D;
    protected Rigidbody2D rb2d
    {
        get
        {
            if (rigidbody2D == null)
            {
                rigidbody2D = GetComponent<Rigidbody2D>();
            }
            return rigidbody2D;
        }
        private set { rigidbody2D = value; }
    }
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
        stats = statBase;
        if (rb2d)
        {
            rb2d.velocity = transform.up * _stats.moveSpeed;
        }
        //TODO: remove this damageable types section when Unity fixes their editor list bug
        //Damagable Types
        damagableTypes = new List<EntityType>();
        if (damagesShots)
        {
            damagableTypes.Add(EntityType.SHOT);
        }
        if (damagesObjects)
        {
            damagableTypes.Add(EntityType.OBJECT);
        }
        if (damagesPlayers)
        {
            damagableTypes.Add(EntityType.PLAYER);
        }
        //HealthPool
        health = GetComponent<HealthPool>();
        health.MaxHealth = _stats.maxHits;
        health.Health = health.MaxHealth;
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
        if (TeamToken.onSameTeam(gameObject, collision.gameObject))
        {
            //don't damage teammates
            return;
        }
        HealthPool hp = collision.gameObject.GetComponent<HealthPool>();
        if (hp)
        {
            if (damagableTypes.Contains(hp.entityType))
            {
                if (useInitialDamage)
                {
                    hp.Health += -_stats.damage;
                }
            }
        }
        else if (!collision.isTrigger)
        {
            if (destroyOnIndestructible)
            {
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
