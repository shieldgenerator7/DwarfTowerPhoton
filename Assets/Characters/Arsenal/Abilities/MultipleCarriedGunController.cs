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

    public bool HoldingShots => carriedShotList.Count > 0;

    public override void OnButtonDown()
    {
        base.OnButtonDown();

        OnButtonHeld();
    }

    public override void OnButtonHeld()
    {
        base.OnButtonHeld();

        //Carry new shot
        if (CanCarryNewShot)
        {
            if (Time.time >= lastCarryNewShotTime + carryNewShotDelay)
            {
                lastCarryNewShotTime = Time.time;
                if (aminaPool.requestAmina(aminaCost) > 0)
                {
                    carryNewShot();
                }
            }
        }
        //Organize carried shots
        if (HoldingShots)
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
        && carriedShotList.Count < maxCarriedShots;
        => aminaPool.hasAmina(aminaCost, true)

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
        registerHealthPoolDelegates(carriedShot, true);
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

    void refreshHealthPoolDelegates(float hp)
    {
        //Remove shots whose hp is 0
        carriedShotList.RemoveAll(
            shot => shot.gameObject.FindComponent<HealthPool>().Health == 0
            );
        //Refresh delegates
        carriedShotList.ForEach(
            shot => registerHealthPoolDelegates(shot, true)
            );
    }
    void registerHealthPoolDelegates(CarryableShot shot, bool register)
    {
        HealthPool hp = shot.gameObject.FindComponent<HealthPool>();
        if (hp)
        {
            hp.onDied -= refreshHealthPoolDelegates;
            if (register)
            {
                hp.onDied += refreshHealthPoolDelegates;
            }
        }
    }

    private void releaseShots()
    {
        if (HoldingShots)
        {
            carriedShotList.ForEach(shot =>
            {
                shot.release();
                registerHealthPoolDelegates(shot, false);
            });
            onReleaseShots?.Invoke(carriedShotList);
            carriedShotList.Clear();
        }
    }
    public delegate void OnReleaseShots(List<CarryableShot> shots);
    public event OnReleaseShots onReleaseShots;
}
