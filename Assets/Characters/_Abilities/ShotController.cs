﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    //
    // Settings
    //
    public float speed = 3;//how fast it travels (units/sec)
    public float initialDamage = 10;//damage dealt upon initial contact
    public float damagePerSecond = 10;//damage per second in seconds
    public float stunDuration = 5;//how long hit players will be stunned for
    public float knockbackDistance = 10;//how far (in total) hit players will be knocked back
    public float maxHealth = 1;//how much health this shot has

    public bool Stuns
    {
        get { return stunDuration > 0; }
    }
    public bool Knocksback
    {
        get { return knockbackDistance > 0; }
    }
    public bool HitsPlayer
    {
        get { return Stuns || Knocksback; }
    }
    public bool HitsObjects
    {
        get { return damagePerSecond > 0; }
    }

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

    private Rigidbody2D rb2d;
    protected PhotonView PV;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d)
        {
            rb2d.velocity = transform.up * speed;
        }
        PV = GetComponent<PhotonView>();
        health = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        processCollision(collision, true);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        processCollision(collision, false);
    }

    void processCollision(Collider2D collision, bool useInitialDamage = false)
    {
        if (TeamToken.onSameTeam(gameObject, collision.gameObject))
        {
            //don't damage teammates
            return;
        }
        if (HitsObjects)
        {
            ShotController sc = collision.gameObject.GetComponent<ShotController>();
            if (sc)
            {
                if (useInitialDamage)
                {
                    sc.addHealth(-initialDamage);
                }
                else
                {
                    sc.addHealth(-damagePerSecond * Time.deltaTime);
                }
            }
        }
        if (HitsPlayer)
        {
            Stunnable stunnable = collision.gameObject.GetComponent<Stunnable>();
            if (stunnable && !stunnable.Stunned)
            {
                stunnable.stun(stunDuration, knockbackDistance);
                if (PV.IsMine)
                {
                    addHealth(-Health);
                }
            }
        }
        if (!collision.isTrigger)
        {
            CaravanController cc = collision.gameObject.GetComponent<CaravanController>();
            if (cc)
            {
                if (PV.IsMine)
                {
                    addHealth(-Health);
                }
            }
        }
    }

    public void addHealth(float health)
    {
        this.Health += health;
    }
}
