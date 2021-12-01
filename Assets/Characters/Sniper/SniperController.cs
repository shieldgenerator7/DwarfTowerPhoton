using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperController : PlayerController
{
    private LaserGunController laser;

    protected override void Start()
    {
        base.Start();

        laser = gameObject.FindComponent<LaserGunController>();
    }

    protected override void onAminaEmpty(float amina)
    {
        if (laser.Active)
        {
            onProcessingFinished -= reload;
            onProcessingFinished += reload;
        }
        else
        {
            reload();
        }
    }

    void reload()
    {
        onProcessingFinished -= reload;
        base.onAminaEmpty(aminaPool.Amina);
    }
}
