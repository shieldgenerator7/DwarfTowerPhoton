using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AminaReloader : PlayerAbility
{
    [Tooltip("Seconds it takes to reload")]
    public float reloadDuration = 2;//time it takes to reload in seconds

    private float reloadStartTime = 0;

    public bool Reloading
    {
        get { return reloadStartTime > 0; }
    }

    public void reload()
    {
        reloadStartTime = Time.time;
        playerController.processAbility(this);
    }

    protected override void Start()
    {
        base.Start();
        aminaPool.onAminaFull += onAminaFull;
    }

    public override void OnButtonDown()
    {
        base.OnButtonDown();
        reload();
    }

    public override void OnContinuedProcessing()
    {
        aminaPool.rechargeAmina(Time.deltaTime * aminaPool.maxAmina / reloadDuration);
    }

    void onAminaFull(float amina)
    {
        reloadStartTime = 0;
        playerController.processAbility(this, false);
    }
}
