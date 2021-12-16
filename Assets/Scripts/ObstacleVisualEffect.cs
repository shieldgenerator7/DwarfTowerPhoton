using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleVisualEffect : MonoBehaviour
{
    private HealthPool healthPool;

    // Start is called before the first frame update
    void Start()
    {
        int sortingOrder = GetComponentInParent<SpriteRenderer>().sortingOrder + 1;
        gameObject.FindComponents<SpriteRenderer>(true)
            .ForEach(sr => sr.sortingOrder = sortingOrder);
        healthPool = gameObject.FindComponent<HealthPool>();
        healthPool.onDamaged += checkOnDamaged;
    }

    void checkOnDamaged(float hp)
    {
        if (hp < healthPool.MaxHealth / 2)
        {
            healthPool.onDamaged -= checkOnDamaged;
            Destroy(gameObject);
        }
    }
}
