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
            maxHealth = Mathf.Clamp(value, 0, value);
            Health = Mathf.Clamp(Health, 0, maxHealth);
        }
    }

    [SerializeField]
    private float health;
    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
            {
                onDied?.Invoke();
            }
        }
    }
    public delegate void OnDied();
    public event OnDied onDied;

}
