using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarriedGunController : PlayerAbility
{
    [Tooltip("Max time until the carried shot reaches max level")]
    public float maxTime = 5;
    [Tooltip("The index of carried shot in the object spawner")]
    public int carriedShotIndex;

    public float CarryTime
    {
        get
        {
            if (carryStartTime > 0)
            {
                return Time.time - carryStartTime;
            }
            return -1;
        }
    }
    private float carryStartTime = -1;

    protected CarriedShotController carriedShot;

    public override void OnButtonDown()
    {
        base.OnButtonDown();

        if (!carriedShot)
        {
            if (rb2d.isMoving())
            {
                if (aminaPool.requestAminaPerSecond(manaCost) > 0)
                {
                    carryNewShot();
                }
            }
        }
    }

    public override void OnButtonHeld()
    {
        base.OnButtonHeld();

        if (carriedShot)
        {
            if (aminaPool.requestAminaPerSecond(manaCost) > 0)
            {
            }
            else
            {
                releaseShot();
            }
            if (!rb2d.isMoving())
            {
                releaseShot();
            }
        }
        else
        {
            if (rb2d.isMoving())
            {
                if (aminaPool.requestAminaPerSecond(manaCost) > 0)
                {
                    carryNewShot();
                }
            }
        }
    }

    public override void OnButtonUp()
    {
        base.OnButtonUp();

        releaseShot();
    }

    private void carryNewShot()
    {
        carryStartTime = Time.time;
        Vector2 spawnPos = playerController.SpawnCenter;
        Vector2 dir = ((Vector2)Utility.MouseWorldPos - spawnPos).normalized;
        carriedShot = objectSpawner.spawnObject<CarriedShotController>(
            carriedShotIndex,
            spawnPos,
            dir
            );
        carriedShot.switchOwner(this);
    }

    private void releaseShot()
    {
        if (carriedShot)
        {
            carriedShot.release();
            carriedShot = null;
            carryStartTime = -1;
        }
    }
}
