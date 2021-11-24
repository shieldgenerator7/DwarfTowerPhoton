using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatKeeper : MonoBehaviour
{
    public StatMatrix selfStats;

    public StatMatrix shotStats;

    public StatMatrix constructStats;

    private void Start()
    {
        selfStats.init();
        shotStats.init();
        constructStats.init();
    }

    public void triggerEvents()
    {
        selfStats.triggerEvent();
        shotStats.triggerEvent();
        constructStats.triggerEvent();
    }
}
