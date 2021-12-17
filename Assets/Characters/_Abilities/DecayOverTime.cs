using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecayOverTime : MonoBehaviour
{
    public float decayPerSecond;

    private HealthPool healthPool;

    // Start is called before the first frame update
    void Start()
    {
        healthPool = gameObject.FindComponent<HealthPool>();
    }

    // Update is called once per frame
    void Update()
    {
        healthPool.Health -= decayPerSecond * Time.deltaTime;
    }
}
