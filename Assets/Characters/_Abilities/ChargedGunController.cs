﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChargedGunController : PlayerAbility
{
    public float aminaReservedPerSecond = 5;//how much amina is reserved per second of charge
    public float exptectedAnimaReserved = 10;
    public float minChargeTime = 1.1f;
    public string shotPrefabName;
    public float spawnBuffer = 1;//how far away from the player the shots spawn
    public bool rotateShot = true;//rotates shot to face the direction it's traveling

    public override void OnButtonDown()
    {
        base.OnButtonDown();
        OnButtonHeld();
    }

    public override void OnButtonHeld()
    {
        base.OnButtonHeld();
        playerController.reserveAmina(aminaReservedPerSecond * Time.deltaTime);
    }

    public override void OnButtonUp()
    {
        base.OnButtonUp();
        if (ButtonHoldDuration >= minChargeTime)
        {
            float aminaObtained = playerController.collectReservedAmina();
            fireShot(
                transform.position,
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                aminaObtained
                );
        }
        else
        {
            playerController.cancelReservedAmina();
        }
    }

    /// <summary>
    /// Fires a shot
    /// 2019-03-20: copied from GunController.fireShot()
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="targetPos"></param>
    public void fireShot(Vector2 playerPos, Vector2 targetPos, float aminaObtained)
    {
        if (PV.IsMine)
        {
            float aminaMultiplier = aminaObtained / exptectedAnimaReserved;
            Vector2 targetDir = (targetPos - playerPos).normalized;
            GameObject shot = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Shots", shotPrefabName),
                playerPos + (targetDir * spawnBuffer),
                (rotateShot) ? Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, targetDir)) : Quaternion.Euler(0, 0, 0)
                );
            shot.GetComponent<ChargedShotController>().chargeStats(aminaMultiplier);

            //On Shot Fired Delegate
            if (onShotFired != null)
            {
                onShotFired(shot, targetPos, targetDir);
            }
        }
    }

    /// <summary>
    /// Reacts to a shot being fired from this gun controller
    /// </summary>
    /// <param name="targetPos">The position targetted by this shot</param>
    /// <param name="targetDir">The direction from the player to the target pos, normalized</param>
    public delegate void OnShotFired(GameObject shot, Vector2 targetPos, Vector2 targetDir);
    public OnShotFired onShotFired;
}
