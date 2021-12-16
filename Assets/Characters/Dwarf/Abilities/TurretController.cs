using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : GunController
{
    public Transform turretBase;

    private ShotController sc;
    private GunController turretFireTrigger;//the gun controller that causes the turrets to fire
    public GunController Trigger
    {
        get { return turretFireTrigger; }
        set
        {
            //Remove previous trigger (if exists)
            if (turretFireTrigger)
            {
                turretFireTrigger.onShotFired -= fireInDirection;
            }
            turretFireTrigger = value;
            //Add new trigger (if exists)
            if (turretFireTrigger)
            {
                turretFireTrigger.onShotFired += fireInDirection;
            }
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sc = GetComponentInParent<ShotController>();
    }

    private void Update()
    {
        if (turretFireTrigger)
        {
            if (PV.IsMine)
            {
                this.transform.up = turretFireTrigger.playerController.LookDirection;
            }
        }
        turretBase.up = Vector2.up;
    }

    private void fireInDirection(GameObject shot, Vector2 targetPos, Vector2 targetDir)
    {
        objectSpawner.spawnObject(
            shotIndex,
            transform.position,
            targetDir
            );
    }

    private void OnDestroy()
    {
        Trigger = null;
    }
}
