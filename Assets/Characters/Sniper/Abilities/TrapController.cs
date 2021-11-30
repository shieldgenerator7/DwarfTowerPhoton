using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : ShotController
{
    [Tooltip("How many seconds an enemy will be trapped for")]
    public float trapDuration = 2;
    [Tooltip("The entity types this trap can trap")]
    public List<EntityType> trapTypes;

    private float trapStartTime = -1;
    private PlayerMovement trappedPlayer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (trapStartTime >= 0)
        {
            if (Time.time >= trapStartTime + trapDuration)
            {
                //untrap player
                trappedPlayer.forceMovement(false);
                //unset startTime
                trapStartTime = -2;
                //destroy trap
                if (PV.IsMine)
                {
                    health.Health = 0;
                }
            }
        }
    }

    protected override void processCollision(Collider2D collision, bool useInitialDamage)
    {
        base.processCollision(collision, useInitialDamage);
        if (trapStartTime != -1)
        {
            //don't trap twice
            return;
        }
        if (TeamToken.onSameTeam(gameObject, collision.gameObject))
        {
            //don't trap teammates
            return;
        }
        HealthPool hp = collision.gameObject.FindComponent<HealthPool>();
        if (hp)
        {
            if (trapTypes.Contains(hp.entityType))
            {
                //TODO: enable trapping other types
                trapPlayer(hp.gameObject.FindComponent<PlayerController>());
            }
        }
    }

    void trapPlayer(PlayerController playerController)
    {
        playerController.cancelAbilities();
        trappedPlayer = playerController.playerMovement;
        trappedPlayer.rb2d.velocity = Vector2.zero;
        trappedPlayer.forceMovement(Vector2.zero);
        trappedPlayer.rb2d.transform.position = transform.position;
        trapStartTime = Time.time;
    }
}
