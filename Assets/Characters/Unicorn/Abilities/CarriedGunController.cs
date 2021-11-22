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

    /// <summary>
    /// How much amina has been consumed since the carried shot was started
    /// </summary>
    public float aminaConsumed { get; private set; }

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
                carryNewShot();
            }
        }
    }

    public override void OnButtonHeld()
    {
        base.OnButtonHeld();

        if (carriedShot)
        {
            aminaConsumed += playerController.requestAminaPerSecond(manaCost);
            if (!rb2d.isMoving())
            {
                releaseShot();
            }
        }
        else
        {
            if (rb2d.isMoving())
            {
                carryNewShot();
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
        aminaConsumed = 0;
        aminaConsumed += playerController.requestAminaPerSecond(manaCost);
        Vector2 dir = ((Vector2)(Utility.MouseWorldPos - transform.position)).normalized;
        carriedShot = objectSpawner.spawnObject<CarriedShotController>(
            carriedShotIndex,
            transform.position,
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
