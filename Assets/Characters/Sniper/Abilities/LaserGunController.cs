using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGunController : PlayerAbility
{
    [Tooltip("The index in the object spawner for the laser")]
    public int laserIndex;

    private LaserShotController laserShotController;
    private bool tryingToShoot = false;
    public bool Active => laserShotController || tryingToShoot;

    public override void OnButtonDown()
    {
        base.OnButtonDown();

        tryingToShoot = true;
        if (aminaPool.requestAmina(aminaCost) > 0)
        {
            activate();
        }
        tryingToShoot = false;
    }

    public override void OnButtonHeld()
    {
        base.OnButtonHeld();

        if (laserShotController)
        {
            updateLaser();
        }
    }

    public override void OnButtonUp()
    {
        base.OnButtonUp();

        if (laserShotController)
        {
            release();
        }
    }
    public override void OnButtonCanceled()
    {
        base.OnButtonCanceled();

        hardDeactivate();
    }

    void activate()
    {
        laserShotController = objectSpawner.spawnObject<LaserShotController>(
            laserIndex,
            playerController.SpawnCenter,
            playerController.LookDirection.normalized
            );
        updateLaser();
    }
    void updateLaser()
    {
        laserShotController.StartPos = playerController.SpawnCenter;
        laserShotController.Direction = playerController.LookDirection;
    }
    void release()
    {
        laserShotController.ReadyToFire = true;
        playerMovement.forceMovement(Vector2.zero);
        laserShotController.onLaserEnded += () =>
        {
            playerMovement.forceMovement(false);
            laserShotController = null;
            playerController.processAbility(this, false);
        };
        playerController.processAbility(this, true);
    }
    void hardDeactivate()
    {
        if (laserShotController)
        {
            laserShotController.destroy();
            playerController.processAbility(this, false);
            laserShotController = null;
        }
    }

    public override void OnContinuedProcessing()
    {
        //do nothing
    }
}
