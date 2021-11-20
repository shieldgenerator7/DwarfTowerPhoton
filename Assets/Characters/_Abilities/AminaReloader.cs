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

    public override void OnButtonDown()
    {
        base.OnButtonDown();
        reload();
    }

    public override void OnContinuedProcessing()
    {
        playerController.rechargeAmina(Time.deltaTime * playerController.maxAmina / reloadDuration);
        if (Time.time > reloadStartTime + reloadDuration
            || playerController.Amina == playerController.maxAmina)
        {
            reloadStartTime = 0;
            playerController.Amina = playerController.maxAmina;
            playerController.processAbility(this, false);
        }
    }
}
