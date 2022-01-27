using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitTakeDamage : MonoBehaviour
{
    public float damage = 1;
    public bool friendlyHit = false;
    public bool onTrigger = true;
    public bool onCollide = true;
    public List<EntityType> entityTypes;

    private HealthPool healthPool;

    private void Start()
    {
        healthPool = gameObject.FindComponent<HealthPool>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (onCollide)
        {
            checkCollision(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (onTrigger)
        {
            checkCollision(collision.gameObject);
        }
    }

    void checkCollision(GameObject go)
    {
        if (friendlyHit || !TeamToken.onSameTeam(gameObject, go))
        {
            HealthPool hp = go.FindComponent<HealthPool>();
            if (hp && entityTypes.Contains(hp.entityType))
            {
                takeDamage();
            }
        }
    }

    void takeDamage()
    {
        healthPool.Health -= damage;
    }
}
