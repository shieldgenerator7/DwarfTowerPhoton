using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MultipleCarriedGunController : PlayerAbility
{
    [Tooltip("The delay between adding new carried shots")]
    public float carryNewShotDelay = 0.5f;
    [Tooltip("The angle in degrees between the shots")]
    public float carryShotAngleBuffer = 11;
    [Tooltip("The max number of carried shots allowed at once")]
    public float maxCarriedShots = 10;
    [Tooltip("The index of carried shot in the object spawner")]
    public int carriedShotIndex;

    private List<CarryableShot> carriedShotList = new List<CarryableShot>();

    private float lastCarryNewShotTime = -1;

    public override void OnButtonDown()
    {
        base.OnButtonDown();

        OnButtonHeld();
    }

    public override void OnButtonHeld()
    {
        base.OnButtonHeld();

        //Carry new shot
        if (lastCarryNewShotTime < 1 ||
            Time.time >= lastCarryNewShotTime + carryNewShotDelay)
        {
            lastCarryNewShotTime = Time.time;
            if (CanCarryNewShot)
            {
                carryNewShot();
            }
        }
        //Organize carried shots
        if (carriedShotList.Count > 0)
        {
            OrganizeShots();
        }
    }

    public override void OnButtonUp()
    {
        base.OnButtonUp();

        releaseShots();
    }

    public override void OnButtonCanceled()
    {
        base.OnButtonCanceled();

        releaseShots();
    }

    private bool CanCarryNewShot
        => aminaPool.requestAmina(aminaCost) > 0
        && carriedShotList.Count < maxCarriedShots;

    private void carryNewShot()
    {
        Vector2 spawnPos = playerController.SpawnCenter;
        Vector2 dir = ((Vector2)Utility.MouseWorldPos - spawnPos).normalized;
        CarryableShot carriedShot = objectSpawner.spawnObject<CarryableShot>(
            carriedShotIndex,
            spawnPos,
            dir
            );
        carriedShot.Start();
        carriedShot.shotController.switchOwner(playerController);
        carriedShotList.Add(carriedShot);
    }

    private void OrganizeShots()
    {
        int count = carriedShotList.Count;
        for (int i = 0; i < count; i++)
        {
            CarryableShot shot = carriedShotList[i];
            shot.holdShotData.holdAngle = (i - (count / 2)) * carryShotAngleBuffer;
        }
    }

    private void releaseShots()
    {
        carriedShotList.ForEach(shot => shot.release());
        onReleaseShots?.Invoke(carriedShotList);
        carriedShotList.Clear();
    }
    public delegate void OnReleaseShots(List<CarryableShot> shots);
    public event OnReleaseShots onReleaseShots;
}
