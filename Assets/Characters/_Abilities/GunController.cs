﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GunController : PlayerAbility
{
    //Settings
    [Tooltip("Seconds between shots")]
    public float fireDelay = 0.1f;//seconds between shots
    [Tooltip("The index of the shot in the object spawner")]
    public int shotIndex;

    //Runtime Vars
    private float lastFireTime = 0;

    public override void OnButtonDown()
    {
        base.OnButtonDown();
        lastFireTime = Mathf.Max(lastFireTime, Time.time - fireDelay);
        OnButtonHeld();
    }
    public override void OnButtonHeld()
    {
        //base.OnButtonHeld();
        if (Time.time > lastFireTime + fireDelay)
        {
            if (manaCost <= 0 || playerController.hasAmina(manaCost))
            {
                lastFireTime = lastFireTime + fireDelay;
                Vector2 dir = (Utility.MouseWorldPos - transform.position).normalized;
                CarriedShotController carriedShot = objectSpawner.spawnObject<CarriedShotController>(
                    shotIndex,
                    transform.position,
                    dir
                    );
                onShotFired?.Invoke(
                    carriedShot.gameObject,
                    carriedShot.transform.position,
                    dir
                    );
            }
        }
    }
    public override void OnButtonUp()
    {
        base.OnButtonUp();
        OnButtonHeld();
    }

    /// <summary>
    /// Reacts to a shot being fired from this gun controller
    /// </summary>
    /// <param name="targetPos">The position targetted by this shot</param>
    /// <param name="targetDir">The direction from the player to the target pos, normalized</param>
    public delegate void OnShotFired(GameObject shot, Vector2 targetPos, Vector2 targetDir);
    public event OnShotFired onShotFired;
}
