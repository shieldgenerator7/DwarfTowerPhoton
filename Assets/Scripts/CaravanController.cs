﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanController : MonoBehaviour
{
    public float maxMoveSpeed = 3;
    private float moveSpeed;
    private Vector2 direction;
    public float maxAllowedDistance = 3;//how far a player can be away but still push it (must still be in trigger)

    public Collider2D detectionColl;//the collider that detects which players are pushing
    private RaycastHit2D[] rch2ds = new RaycastHit2D[100];//used for detection

    public MapMarkerInfo caravanMarkerInfo;

    public SpriteRenderer contestEffect;

    public MapPathGenerator pathGenerator;

    private Dictionary<TeamTokenCaptain, float> teamCaptains = new Dictionary<TeamTokenCaptain, float>();

    private static CaravanController instance;
    public static CaravanController Caravan
    {
        get { return instance; }
    }

    private float distanceFromStart;

    private PhotonView PV;
    private Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        rb2d = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
        foreach (TeamTokenCaptain ttc in FindObjectsOfType<TeamTokenCaptain>())
        {
            teamCaptains.Add(ttc, 0);
        }
        updateDirection();
        contestEffect.enabled = false;
        pathGenerator.onMapPathGenerated += updatePositionOnPath;
        if (pathGenerator.mapPath != null)
        {
            updatePositionOnPath(pathGenerator.mapPath);
        }
        //Marker
        FindObjectOfType<MapMarkerManager>().CreateMapMarker(
            PhotonView.Get( gameObject),
            transform,
            caravanMarkerInfo
            );
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            updatePushingPlayers();
        }
    }

    void updatePushingPlayers()
    {
        teamCaptains.Clear();
        int count = detectionColl.Cast(Vector2.zero, rch2ds, 0, true);
        for (int i = 0; i < count; i++)
        {
            RaycastHit2D rch2d = rch2ds[i];
            GameObject rchGO = rch2d.collider.gameObject;
            if (rchGO.CompareTag("Player"))
            {
                TeamToken tt = TeamToken.getTeamToken(rchGO);
                Stunnable stunnable = rchGO.GetComponentInChildren<Stunnable>();
                if (!stunnable.Stunned)
                {
                    if (!teamCaptains.ContainsKey(tt.teamCaptain))
                    {
                        teamCaptains.Add(tt.teamCaptain, 0);
                    }
                    float amount = 1;
                    //Add in distance to caravan
                    float distance = (rchGO.transform.position - transform.position).magnitude;
                    float distanceMultiplier = 1 + ((maxAllowedDistance - distance) / maxAllowedDistance);
                    //Update team captains dict
                    teamCaptains[tt.teamCaptain] += amount * distanceMultiplier;
                }
            }
        }
        updateDirection();
    }

    void updateDirection()
    {
        direction = Vector2.zero;
        foreach (TeamTokenCaptain ttc in teamCaptains.Keys)
        {
            direction += (Vector2)(transform.position - ttc.transform.position).normalized
                * teamCaptains[ttc];
        }
        if (pathGenerator.mapPath != null)
        {
            float magnitude = direction.magnitude;
            magnitude = Mathf.Clamp(magnitude, 0, maxMoveSpeed);
            distanceFromStart += direction.normalized.y * magnitude * Time.deltaTime;
            Vector2 desiredPos = pathGenerator.mapPath.getPosition(distanceFromStart);
            direction = (desiredPos - (Vector2)transform.position).normalized;
            rb2d.velocity = direction * magnitude;
        }
        else
        {
            rb2d.velocity = direction;
        }
    }

    void updatePositionOnPath(MapPath mapPath)
    {
        distanceFromStart = mapPath.Length / 2;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Checking if the game should end (when the caravan hits a team flag)
        if (collision.gameObject.CompareTag("TeamFlag"))
        {
            //Turn off the caravan for good
            Destroy(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Show contest effect only for the contesting player
        PlayerController pc = collision.gameObject.FindComponent<PlayerController>();
        if (pc && pc.PV.IsMine && !pc.stunnable.Stunned)
        {
            contestEffect.enabled = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Show contest effect only for the contesting player
        PlayerController pc = collision.gameObject.FindComponent<PlayerController>();
        if (pc && pc.PV.IsMine)
        {
            contestEffect.enabled = !pc.stunnable.Stunned;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Hide contest effect only for the contesting player
        PlayerController pc = collision.gameObject.FindComponent<PlayerController>();
        if (pc && pc.PV.IsMine)
        {
            contestEffect.enabled = false;
        }
    }
}
