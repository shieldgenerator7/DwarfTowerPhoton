using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretLauncher : ChargedGunController
{
    public GunController turretFireTrigger;//the gun controller that causes the turrets to fire

    protected override void Start()
    {
        base.Start();
        onShotFired += addTurretTrigger;
    }

    public void addTurretTrigger(GameObject shot, Vector2 targetPos, Vector2 targetDir)
    {
        turretFireTrigger.onShotFired += shot.GetComponentInChildren<TurretController>().fireInDirection;
    }
}
