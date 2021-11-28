using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [Tooltip("How much damage to deal")]
    public float damage;

    [Tooltip("The EntityTypes that this damager can damage")]
    public List<EntityType> damagableTypes;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        processCollision(collision, true);
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    processCollision(collision, false);
    //}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        processCollision(collision.collider, true);
    }
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    processCollision(collision.collider, false);
    //}

    protected virtual void processCollision(Collider2D collision, bool useInitialDamage)
    {
        if (TeamToken.onSameTeam(gameObject, collision.gameObject))
        {
            //don't damage teammates
            return;
        }
        HealthPool hp = collision.gameObject.GetComponent<HealthPool>();
        if (hp)
        {
            if (damagableTypes.Contains(hp.entityType))
            {
                if (useInitialDamage)
                {
                    hp.Health += -damage;
                }
            }
        }
    }
}
