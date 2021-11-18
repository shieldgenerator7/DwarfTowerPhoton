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
    private StatLayer _stats;

    public bool hitsPlayer = true;
    public bool hitsObjects = true;

    //
    // Runtime Vars
    //

    [SerializeField]
    private float health;
    protected float Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
            {
                if (PV.IsMine)
                {
                    PhotonNetwork.Destroy(this.gameObject);
                }
                else
                {
                    GetComponent<Collider2D>().enabled = false;
                }
            }
        }
    }

    //
    // Components
    //

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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (rb2d)
        {
            rb2d.velocity = transform.up * _stats.moveSpeed;
        }
        health = _stats.maxHits;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        processCollision(collision, true);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        processCollision(collision, false);
    }

    protected void processCollision(Collider2D collision, bool useInitialDamage)
    {
        if (TeamToken.onSameTeam(gameObject, collision.gameObject))
        {
            //don't damage teammates
            return;
        }
        if (hitsObjects)
        {
            ShotController sc = collision.gameObject.GetComponent<ShotController>();
            if (sc)
            {
                if (useInitialDamage)
                {
                    sc.addHealth(-_stats.damage);
                }
            }
        }
        if (hitsPlayer)
        {
            Stunnable stunnable = collision.gameObject.GetComponent<Stunnable>();
            if (stunnable && !stunnable.Stunned)
            {
                PhotonView targetView = collision.gameObject.GetComponentInParent<PhotonView>();
                if (PV.IsMine)
                {
                    PV.RPC("RPC_StunTarget", RpcTarget.All, targetView.ViewID);
                }
            }
        }
        if (!collision.isTrigger)
        {
            CaravanController cc = collision.gameObject.GetComponent<CaravanController>();
            if (cc)
            {
                Health = 0;
            }
        }
    }

    public void addHealth(float health)
    {
        this.Health += health;
    }

    [PunRPC]
    protected void RPC_StunTarget(int targetID)
    {
        foreach (PhotonView targetView in FindObjectsOfType<PhotonView>())
        {
            if (targetView.ViewID == targetID)
            {
                Stunnable stunnable = targetView.gameObject.GetComponentInChildren<Stunnable>();
                if (!stunnable.Stunned)
                {
                    stunnable.stun();
                    if (targetView.IsMine)
                    {
                        PV.RPC("RPC_SelfDestruct", RpcTarget.All);
                    }
                }
                break;
            }
        }
    }

    [PunRPC]
    protected void RPC_SelfDestruct()
    {
        Health = 0;
    }
}
