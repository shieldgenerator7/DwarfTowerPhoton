using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarriedGunController : PlayerAbility
{
    [Tooltip("Max time until the carried shot reaches max level")]
    public float maxTime = 5;
    [Tooltip("The name of the prefab to spawn from this character's folder under Resources/PhotonPrefabs/Shots")]
    public string shotPrefabName;
    /// <summary>
    /// The name of the subfolder of Resources/PhotonPrefabs/Shots that this is from
    /// null or "": defaults to parent gameObject's name
    /// </summary>
    [Tooltip("The name of this character. Leave blank to default to parent GameObject's name")]
    public string subfolderName;
    [Tooltip("How far away from the player the shots spawn when released with \"normal\" values")]
    public float spawnBuffer = 1;//how far away from the player the shots spawn
    [Tooltip("Should the shot rotate to face the direction it's traveling?")]
    public bool rotateShot = true;//rotates shot to face the direction it's traveling

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

    protected override void Start()
    {
        base.Start();
        if (PV.IsMine)
        {
            //Subfoldername
            if (string.IsNullOrEmpty(subfolderName))
            {
                string name = transform.parent.gameObject.name;
                name = name.Replace("(Clone)", "").Trim();
                subfolderName = name;
            }
        }
    }

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
        carriedShot = fireShot(
            transform.position,
            Utility.MouseWorldPos
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

    /// <summary>
    /// Fires a shot
    /// 2019-03-20: copied from GunController.fireShot()
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="targetPos"></param>
    public CarriedShotController fireShot(Vector2 playerPos, Vector2 targetPos)
    {
        if (PV.IsMine)
        {
            Vector2 targetDir = (targetPos - playerPos).normalized;
            GameObject shot = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Shots", subfolderName, shotPrefabName),
                playerPos + (targetDir * spawnBuffer),
                (rotateShot) ? Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, targetDir)) : Quaternion.Euler(0, 0, 0)
                );

            //On Shot Fired Delegate
            onShotFired?.Invoke(shot, targetPos, targetDir);
            //Return
            return shot.GetComponent<CarriedShotController>();
        }
        return null;
    }

    /// <summary>
    /// Reacts to a shot being fired from this gun controller
    /// </summary>
    /// <param name="targetPos">The position targetted by this shot</param>
    /// <param name="targetDir">The direction from the player to the target pos, normalized</param>
    public delegate void OnShotFired(GameObject shot, Vector2 targetPos, Vector2 targetDir);
    public OnShotFired onShotFired;
}
