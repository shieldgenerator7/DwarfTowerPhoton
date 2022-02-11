using Photon.Pun;
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
    [Tooltip("Should the shot be made aware of who its owner is?")]
    public bool shouldSetOwner = false;

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
                Vector2 dir = ((Vector2)Utility.MouseWorldPos - spawnPos);
                if (angle != 0)
                {
                    dir = Quaternion.Euler(0, 0, angle) * dir;
                }
                dir.Normalize();
                //Launch shot
                ShotController shot = objectSpawner.spawnObject<ShotController>(
                    shotIndex,
                    spawnPos,
                    dir
                    );
                onShotFired?.Invoke(
                    shot.gameObject,
                    shot.transform.position,
                    dir
                    );
                if (shouldSetOwner)
                {
                    shot.teamToken.switchController(playerController.teamToken);
                }
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
