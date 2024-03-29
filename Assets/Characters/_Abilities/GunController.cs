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
    [Tooltip("Angle from the cursor")]
    public float angle = 0;//angle from the cursor
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
            if (aminaCost <= 0 || aminaPool.requestAmina(aminaCost) > 0)
            {
                Vector2 spawnPos = playerController.SpawnCenter;
                lastFireTime = lastFireTime + fireDelay;
                //prevent spam burst bug
                if (Mathf.Abs(Time.time - lastFireTime) > Time.deltaTime)
                {
                    lastFireTime = Time.time;
                }
                //Determine direction
                Vector2 targetPos = Utility.MouseWorldPos;
                Vector2 dir = (targetPos - spawnPos);
                if (angle != 0)
                {
                    dir = Quaternion.Euler(0, 0, angle) * dir;
                }
                dir.Normalize();
                //Launch shot
                GameObject shot = objectSpawner.spawnObject(
                    shotIndex,
                    spawnPos,
                    dir
                    );
                RuleProcessor rp = shot.FindComponent<RuleProcessor>();
                if (rp)
                {
                    rp.Init(dir, targetPos);
                }
                onShotFired?.Invoke(
                    shot,
                    shot.transform.position,
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
