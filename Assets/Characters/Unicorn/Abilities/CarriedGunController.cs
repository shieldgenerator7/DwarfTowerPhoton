using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarriedGunController : PlayerAbility
{
    [Tooltip("The index of carried shot in the object spawner")]
    public int carriedShotIndex;

    protected CarriedShotController carriedShot;

    public override void OnButtonDown()
    {
        base.OnButtonDown();

        if (!carriedShot)
        {
            if (aminaPool.requestAminaPerSecond(aminaCost) > 0)
            {
                carryNewShot();
            }
        }
    }

    public override void OnButtonHeld()
    {
        base.OnButtonHeld();

        if (aminaPool.requestAminaPerSecond(aminaCost) > 0)
        {
            if (!carriedShot)
            {
                carryNewShot();
            }
        }
        else
        {
            if (carriedShot)
            {
                releaseShot();
            }
        }
    }

    public override void OnButtonUp()
    {
        base.OnButtonUp();

        releaseShot();
    }

    public override void OnButtonCanceled()
    {
        base.OnButtonCanceled();

        releaseShot();
    }

    private void carryNewShot()
    {
        Vector2 spawnPos = playerController.SpawnCenter;
        Vector2 dir = ((Vector2)Utility.MouseWorldPos - spawnPos).normalized;
        carriedShot = objectSpawner.spawnObject<CarriedShotController>(
            carriedShotIndex,
            spawnPos,
            dir
            );
        carriedShot.teamToken.switchController(playerController.teamToken);
    }

    private void releaseShot()
    {
        if (carriedShot)
        {
            carriedShot.release();
            carriedShot = null;
        }
    }
}
