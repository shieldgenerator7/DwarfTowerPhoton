using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [Tooltip("How much damage to deal")]
    public float damage = 1;

    [Tooltip("The EntityTypes that this damager can damage")]
    public List<EntityType> damagableTypes;

    [Tooltip("Should this damage the object when it comes into this trigger?")]
    public bool damageOnTrigger = true;

    [Tooltip("Should this damage the object when it collides with this?")]
    public bool damageOnCollide = true;

    [Tooltip("Should this damage the object even if it's on the same team?")]
    public bool damageFriendlies = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (damageOnTrigger)
        {
            processCollision(collision, true);
        }
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    processCollision(collision, false);
    //}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (damageOnCollide)
        {
            processCollision(collision.collider, true);
        }
    }
    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    processCollision(collision.collider, false);
    //}

    public void processCollision(Collider2D collision, bool useInitialDamage)
    {
        if (!damageFriendlies && TeamToken.onSameTeam(gameObject, collision.gameObject))
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
                    DealDamage(hp);
                }
            }
        }
    }

    public void DealDamage(HealthPool hp)
    {
        hp.Health += -damage;
        onDealtDamage?.Invoke(damage, hp);
    }
    public delegate void OnDealtDamage(float damage, HealthPool healthPool);
    public event OnDealtDamage onDealtDamage;
}
