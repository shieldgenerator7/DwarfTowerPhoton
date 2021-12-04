using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPool : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The maximum amount of health this entity can have")]
    private float maxHealth;
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            //TODO: What does it mean for max health to be 0??
            float newMaxHealth = Mathf.Clamp(value, 0, value);
            float diff = newMaxHealth - maxHealth;
            if (diff > 0)
            {
                //Increase health with max health increase
                Health += diff;
            }
            else
            {
                //Make sure health doesn't end up above max health
                Health += 0;
            }
        }
    }

    /// <summary>
    /// The current amount of health this entity has
    /// </summary>
    public float Health
    {
        get => health;
        set
        {
            float prevHealth = health;
            health = Mathf.Clamp(value, 0, maxHealth);
            if (health < prevHealth)
            {
                onDamaged?.Invoke();
            }
            if (health <= 0)
            {
                onDied?.Invoke();
            }
        }
    }
    private float health;
    public delegate void HealthEvent();
    public event HealthEvent onDamaged;
    public event HealthEvent onDied;

    [Tooltip("The type of entity this health pool represents")]
    public EntityType entityType;

    public void Start()
    {
        //Set initial health value
        health = maxHealth;
        //Check for rb2d
        //rb2d (even a static one) needed for some hit detection to work (like Sniper's laser)
        Rigidbody2D rb2d = gameObject.FindComponent<Rigidbody2D>();
        if (!rb2d)
        {
            Debug.LogError(
                $"HealthPool on object {name} does not have a Rigidbody2D!",
                gameObject
                );
        }
    }

}
