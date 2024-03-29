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
    public MapMarkerInfo trapMarkerInfo;

    private ComponentContext trappedPlayer;

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
                trapPlayer(hp.gameObject.FindComponent<ComponentContext>());
            }
        }
    }

    void trapPlayer(ComponentContext componentContext)
    {
        componentContext.statusKeeper.addLayer(
            PV.ViewID,
            new StatusLayer(StatusEffect.ROOTED)
            );
        trappedPlayer = componentContext;
        trappedPlayer.playerMovement.rb2d.transform.position = transform.position;
        //Map marker
        MapMarkerManager.CreateMapMarker(
            PV,
            (Vector2)transform.position + (Vector2.up * 0.5f),
            trapMarkerInfo
            );
        TimerManager.StartTimer(trapDuration, () =>
        {
            //untrap player
            trappedPlayer.statusKeeper.removeLayer(PV.ViewID);
            //destroy trap
            if (PV.IsMine)
            {
                health.Health = 0;
            }
            //Make it disappear even if it doesn't actually get destroyed
            gameObject.FindComponent<SpriteRenderer>().enabled = false;
            gameObject.FindComponent<Collider2D>().enabled = false;
            this.enabled = false;
            //Destroy marker
            MapMarkerManager.DestroyMapMarker(PV);
        });
    }
}
