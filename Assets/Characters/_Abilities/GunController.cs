using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GunController : PlayerAbility
{
    //Settings
    public float fireDelay = 0.1f;//seconds between shots
    public string shotPrefabName;
    public float spawnBuffer = 1;//how far away from the player the shots spawn

    //Runtime Vars
    private float lastFireTime = 0;

    protected override void Start()
    {
        base.Start();
    }

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
            lastFireTime = lastFireTime + fireDelay;
            fireShot(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
    public override void OnButtonUp()
    {
        base.OnButtonUp();
        OnButtonHeld();
    }

    public void fireShot(Vector2 playerPos, Vector2 targetPos)
    {
        if (PV.IsMine)
        {
            Vector2 targetDir = (targetPos - playerPos).normalized;
            GameObject shot = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Shots", shotPrefabName),
                playerPos + (targetDir * spawnBuffer),
                Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, targetDir))
                );

            //shot.transform.position = ;
            //shot.transform.up = ;

            //On Shot Fired Delegate
            if (onShotFired != null)
            {
                onShotFired(targetPos, targetDir);
            }
        }
    }

    /// <summary>
    /// Reacts to a shot being fired from this gun controller
    /// </summary>
    /// <param name="targetPos">The position targetted by this shot</param>
    /// <param name="targetDir">The direction from the player to the target pos, normalized</param>
    public delegate void OnShotFired(Vector2 targetPos, Vector2 targetDir);
    public OnShotFired onShotFired;

}
