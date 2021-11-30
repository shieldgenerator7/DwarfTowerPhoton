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
                //destroy trap
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    protected override void processCollision(Collider2D collision, bool useInitialDamage)
    {
        base.processCollision(collision, useInitialDamage);
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
                trappedPlayer = hp.gameObject.FindComponent<PlayerMovement>();
                trappedPlayer.forceMovement(Vector2.zero);
                trappedPlayer.rb2d.transform.position = transform.position;
                trapStartTime = Time.time;
            }
        }
    }
}
