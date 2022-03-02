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
            onStatsChanged?.Invoke(_stats);
        }
    }
    private StatLayer _stats;
    public delegate void OnStatsChanged(StatLayer stats);
    public event OnStatsChanged onStatsChanged;

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

    private PlayerController _controller;
    public PlayerController Controller
    {
        get => _controller;
        set => teamToken.switchController(value?.context.teamToken);
    }

    protected Damager damager { get; private set; }
    protected TeamToken teamToken { get; private set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        stats = statBase;
        Launch();
        //HealthPool
        health = GetComponent<HealthPool>();
        if (health)
        {
            health.MaxHealth = _stats.maxHits;
            health.Health = health.MaxHealth;
        }
        //Damage
        damager = gameObject.FindComponent<Damager>();
        //TeamToken
        teamToken = gameObject.FindComponent<TeamToken>();
        teamToken.onControllerGainedControl += updateController;
        updateController(teamToken.controller);
        //Delegates
        onStatsChanged -= updateFromStats;
        onStatsChanged += updateFromStats;
        updateFromStats(stats);
    }

    public void Launch()
    {
        if (rb2d)
        {
            rb2d.velocity = transform.up * _stats.moveSpeed;
        }
    }

    private void updateController(TeamToken controller)
    {
        this._controller = controller.gameObject.FindComponent<PlayerController>();
    }

    protected virtual void updateFromStats(StatLayer statLayer)
    {
        //TODO: Implement all these stats, and more
        ////Move Speed
        //if (statLayer.moveSpeed != StatLayer.STAT_IGNORE)
        //{
        //    if (rb2d)
        //    {
        //        rb2d.velocity = rb2d.velocity.normalized * statLayer.moveSpeed;
        //    }
        //}
        //HealthPool
        if (health)
        {
            if (statLayer.maxHits != StatLayer.STAT_IGNORE)
            {
                health.MaxHealth = statLayer.maxHits;
            }
        }
        //Damage
        if (damager && statLayer.damage != StatLayer.STAT_IGNORE)
        {
            damager.damage = statLayer.damage;
        }
        ////Size
        //if (statLayer.size != StatLayer.STAT_IGNORE)
        //{
        //    transform.localScale = Vector3.one * statLayer.size;
        //}
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
}
