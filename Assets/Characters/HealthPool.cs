using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPool : MonoBehaviour
{
    [SerializeField]
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

    [SerializeField]
    private float health;
    public float Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, maxHealth);
            if (health <= 0)
            {
                onDied?.Invoke();
            }
        }
    }
    public delegate void OnDied();
    public event OnDied onDied;

}
