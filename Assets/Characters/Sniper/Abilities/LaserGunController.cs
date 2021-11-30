using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGunController : PlayerAbility
{
    [Tooltip("The index in the object spawner for the laser")]
    public int laserIndex;

    private LaserShotController laserShotController;

    public override void OnButtonDown()
    {
        base.OnButtonDown();

        if (aminaPool.requestAmina(manaCost) > 0)
        {
            activate();
        }
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
            if (PV.IsMine)
            {
                PhotonNetwork.DestroyAll(laserShotController.PV.gameObject);
            }
            playerController.processAbility(this, false);
        }
    }

    public override void OnContinuedProcessing()
    {
        //do nothing
    }
}
