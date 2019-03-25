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
    /// <summary>
    /// //the name of the subfolder of Resources/PhotonPrefabs/Shots that this is from
    /// "!parent": get the subfolder name from the parent of this GameObject
    /// "!this": get the subfolder name from this GameObject
    /// null or "": defaults to parent
    /// </summary>
    public string subfolderName = "!parent";
    public float spawnBuffer = 1;//how far away from the player the shots spawn
    public bool rotateShot = true;//rotates shot to face the direction it's traveling

    //Runtime Vars
    private float lastFireTime = 0;

    protected override void Start()
    {
        base.Start();
        if (subfolderName == null || subfolderName == "")
        {
            subfolderName = "!parent";
        }
        if (subfolderName == "!parent")
        {
            subfolderName = transform.parent.gameObject.name.Replace("(Clone)", "");
        }
        else if (subfolderName == "!this")
        {
            subfolderName = gameObject.name.Replace("(Clone)", "");
        }
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
            if (manaCost <= 0 || playerController.hasAmina(manaCost))
            {
                lastFireTime = lastFireTime + fireDelay;
                fireShot(transform.position, Utility.MouseWorldPos);
            }
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
            float aminaMultiplier = (manaCost > 0) ? playerController.requestAmina(manaCost) / manaCost : 1;
            Vector2 targetDir = (targetPos - playerPos).normalized;
            GameObject shot = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Shots", subfolderName, shotPrefabName),
                playerPos + (targetDir * spawnBuffer),
                (rotateShot) ? Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, targetDir)) : Quaternion.Euler(0, 0, 0)
                );

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
