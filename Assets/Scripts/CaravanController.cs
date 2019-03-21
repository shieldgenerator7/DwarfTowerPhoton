using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanController : MonoBehaviour
{
    public float maxMoveSpeed = 3;
    private float moveSpeed;
    private Vector2 direction;
    public float maxAllowedDistance = 3;//how far a player can be away but still push it (must still be in trigger)
    public float stunDuration = 1;
    public float knockbackDistance = 5;

    public Collider2D detectionColl;//the collider that detects which players are pushing
    private RaycastHit2D[] rch2ds = new RaycastHit2D[100];//used for detection

    private Dictionary<TeamToken, float> teamCaptains = new Dictionary<TeamToken, float>();

    private static CaravanController instance;
    public static CaravanController Caravan
    {
        get { return instance; }
    }

    private PhotonView PV;
    private Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        rb2d = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
        foreach (TeamToken tt in FindObjectsOfType<TeamToken>())
        {
            if (!teamCaptains.ContainsKey(tt.teamCaptain))
            {
                teamCaptains.Add(tt.teamCaptain, 0);
            }
        }
        updateDirection();
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
        foreach (TeamToken tt in teamCaptains.Keys)
        {
            direction += (Vector2)(transform.position - tt.transform.position).normalized
                * teamCaptains[tt];
        }
        float magnitude = direction.magnitude;
        magnitude = Mathf.Clamp(magnitude, 0, maxMoveSpeed);
        rb2d.velocity = direction.normalized * magnitude;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ShotController sc = collision.gameObject.GetComponent<ShotController>();
        if (sc)
        {
            sc.addHealth(-sc.maxHealth);
        }
        Stunnable stunnable = collision.gameObject.GetComponentInChildren<Stunnable>();
        if (stunnable && !stunnable.Stunned)
        {
            stunnable.stun(stunDuration, knockbackDistance);
        }
    }
}
