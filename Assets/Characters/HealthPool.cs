using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPool : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The maximum amount of health this entity can have")]
    private float maxHealth = 1;
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            float prevMaxHealth = maxHealth;
            //TODO: What does it mean for max health to be 0??
            maxHealth = Mathf.Clamp(value, 0, value);
            float diff = maxHealth - prevMaxHealth;
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
            onMaxHealthChanged?.Invoke(maxHealth);
        }
    }
    public event HealthEvent onMaxHealthChanged;

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
            if (prevHealth != health)
            {
                onChanged?.Invoke(health);
                //Healed
                if (health > prevHealth)
                {
                    if (health > 0)
                    {
                        onHealed?.Invoke(health);
                    }
                    if (health >= maxHealth)
                    {
                        onHealedFull?.Invoke(health);
                    }
                }
                //Damaged (or max health decreased)
                else if (health < prevHealth)
                {
                    if (health < maxHealth)
                    {
                        onDamaged?.Invoke(health);
                    }
                    else
                    {
                        //when decreasing max health,
                        //can make health equal to max health,
                        //thus healing to full
                        onHealedFull?.Invoke(health);
                    }
                    if (health <= 0)
                    {
                        onDied?.Invoke(health);
                    }
                }
            }
        }
    }
    private float health;
    public delegate void HealthEvent(float health);
    public event HealthEvent onChanged;
    public event HealthEvent onHealed;
    public event HealthEvent onHealedFull;
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
