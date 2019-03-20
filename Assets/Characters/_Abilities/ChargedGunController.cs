using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChargedGunController : PlayerAbility
{
    public float aminaReservedPerSecond = 5;//how much amina is reserved per second of charge
    public float expectedAnimaReserved = 10;
    public float minAminaReserved = 5.1f;
    public string shotPrefabName;
    public float defaultSpawnBuffer = 1;//how far away from the player the shots spawn
    public float minSpawnBuffer = 0;
    public float maxSpawnBuffer = 2;
    public bool rotateShot = true;//rotates shot to face the direction it's traveling
    public GameObject previewPrefab;

    public float SpawnBuffer
    {
        get
        {
            if (preview)
            {
                Vector2 playerPos = transform.position;
                Vector2 targetPos = Utility.MouseWorldPos;
                Vector2 targetDir = (targetPos - playerPos);
                float magnitude = targetDir.magnitude;
                magnitude = Mathf.Clamp(magnitude, minSpawnBuffer, maxSpawnBuffer);
                return magnitude;
            }
            else
            {
                return defaultSpawnBuffer;
            }
        }
    }

    private GameObject preview;
    private Collider2D previewCollider;
    private PreviewDisplayer previewDisplayer;

    protected override void Start()
    {
        base.Start();
        if (previewPrefab)
        {
            preview = Instantiate(previewPrefab);
            preview.SetActive(false);
            previewCollider = preview.GetComponent<Collider2D>();
            previewDisplayer = preview.GetComponent<PreviewDisplayer>();
        }
    }

    public override void OnButtonDown()
    {
        base.OnButtonDown();
        if (preview)
        {
            preview.SetActive(true);
        }
        OnButtonHeld();
    }

    public override void OnButtonHeld()
    {
        base.OnButtonHeld();
        playerController.reserveAmina(aminaReservedPerSecond * Time.deltaTime);
        //Check to see if the preview collides with anything
        if (preview)
        {
            PreviewDisplayer.PreviewState state = getPreviewState();
            previewDisplayer.updatePreview(state);
        }
    }

    public override void OnButtonUp()
    {
        base.OnButtonUp();
        bool previewHasClearLanding = true;
        if (preview)
        {
            PreviewDisplayer.PreviewState state = getPreviewState();
            previewHasClearLanding = state == PreviewDisplayer.PreviewState.BUILD;
        }
        if (playerController.ReservedAmina >= minAminaReserved
            && previewHasClearLanding)
        {
            float aminaObtained = playerController.collectReservedAmina();
            fireShot(
                transform.position,
                Utility.MouseWorldPos,
                aminaObtained
                );
        }
        else
        {
            playerController.cancelReservedAmina();
        }
        if (preview)
        {
            preview.SetActive(false);
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
            float aminaMultiplier = aminaObtained / expectedAnimaReserved;
            Vector2 targetDir = (targetPos - playerPos).normalized;
            GameObject shot = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Shots", shotPrefabName),
                playerPos + (targetDir * SpawnBuffer),
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

    /// <summary>
    /// Returns the preview state of the updated preview location
    /// </summary>
    /// <returns></returns>
    private PreviewDisplayer.PreviewState getPreviewState()
    {
        Vector2 playerPos = transform.position;
        Vector2 targetPos = Utility.MouseWorldPos;
        Vector2 targetDir = (targetPos - playerPos).normalized;
        Vector2 pos = playerPos + (targetDir * SpawnBuffer);
        return getPreviewState(pos);
    }

    private PreviewDisplayer.PreviewState getPreviewState(Vector2 position)
    {
        preview.transform.position = position;
        GameObject conflictingObject = null;
        bool coHasRB2D = false;
        bool coHasSC = false;
        RaycastHit2D[] rch2ds = new RaycastHit2D[10];
        int count = previewCollider.Cast(Vector2.zero, rch2ds, 0, true);
        for (int i = 0; i < count; i++)
        {
            RaycastHit2D rch2d = rch2ds[i];
            Rigidbody2D rchRB2D = rch2d.collider.gameObject.GetComponent<Rigidbody2D>();
            ShotController rchSC = rch2d.collider.gameObject.GetComponent<ShotController>();
            //If the conflicting object is a regular moving shot,
            if (rchRB2D && rchSC)
            {
                //You can build here anyway
                continue;
            }
            //If the conflicting object is non-moving or is not a shot,
            else
            {
                //it's conflicting
                conflictingObject = rch2d.collider.gameObject;
                coHasRB2D = rchRB2D;
                coHasSC = rchSC;
                break;
            }
        }
        if (conflictingObject)
        {
            return PreviewDisplayer.PreviewState.NONE;
        }
        return PreviewDisplayer.PreviewState.BUILD;
    }
}
