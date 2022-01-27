using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDieExplode : MonoBehaviour
{
    public float explodeRange = 1;
    public float explodePower = 10;
    public float forceMoveDuration = 0.2f;
    public List<EntityType> entityTypes;

    private HealthPool healthPool;

    // Start is called before the first frame update
    void Start()
    {
        healthPool = gameObject.FindComponent<HealthPool>();
        healthPool.onDied += (hp) => explode();
    }

    void explode()
    {
        List<Rigidbody2D> rb2ds = new List<Rigidbody2D>();
        RaycastHit2D[] rch2ds = Physics2D.CircleCastAll(transform.position, explodeRange, Vector2.zero);
        for (int i = 0; i < rch2ds.Length; i++)
        {
            Rigidbody2D rb2d = rch2ds[i].rigidbody;
            if (!rb2d)
            {
                continue;
            }
            if (!rb2ds.Contains(rb2d))
            {
                rb2ds.Add(rb2d);
            }
        }
        foreach (Rigidbody2D rb2d in rb2ds)
        {
            HealthPool hp = rb2d.gameObject.FindComponent<HealthPool>();
            if (hp && entityTypes.Contains(hp.entityType))
            {
                Vector2 explodeDir = (rb2d.transform.position - transform.position).normalized;
                Vector2 explodeVector = explodeDir * explodePower;
                rb2d.velocity = explodeVector;
                //PlayerMovement
                PlayerMovement playerMovement = rb2d.gameObject.FindComponent<PlayerMovement>();
                if (playerMovement)
                {
                    //TODO: refactor this once MovementLayer has been implemented
                    float speed = explodePower / playerMovement.MovementSpeed;
                    playerMovement.forceMovement(explodeVector, true);
                    int viewID = gameObject.FindComponent<PhotonView>().ViewID;
                    StatLayer statLayer = new StatLayer(StatLayer.STAT_IGNORE)
                    {
                        moveSpeed = speed
                    };
                    StatKeeper statKeeper = playerMovement.gameObject.FindComponent<StatKeeper>();
                    statKeeper.selfStats.addLayer(viewID, statLayer);
                    TimerManager.StartTimer(forceMoveDuration, () =>
                    {
                        playerMovement.forceMovement(false);
                        statKeeper.selfStats.removeLayer(viewID);
                    });
                }
            }
        }
    }
}
