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
    [Tooltip("The name of the prefab to spawn from this character's folder under Resources/PhotonPrefabs/Shots")]
    public string shotPrefabName;
    /// <summary>
    /// The name of the subfolder of Resources/PhotonPrefabs/Shots that this is from
    /// null or "": defaults to parent gameObject's name
    /// </summary>
    [Tooltip("The name of this character. Leave blank to default to parent GameObject's name")]
    public string subfolderName;
    [Tooltip("How far away from the player the shots spawn")]
    public float spawnBuffer = 1;//how far away from the player the shots spawn
    [Tooltip("Should the shot rotate to face the direction it's traveling?")]
    public bool rotateShot = true;//rotates shot to face the direction it's traveling

    //Runtime Vars
    private float lastFireTime = 0;

    protected override void Start()
    {
        base.Start();
        //Subfoldername
        if (string.IsNullOrEmpty(subfolderName))
        {
            string name = transform.parent.gameObject.name;
            name = name.Replace("(Clone)", "").Trim();
            subfolderName = name;
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
