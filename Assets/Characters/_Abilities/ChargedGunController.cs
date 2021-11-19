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
    /// <summary>
    /// The name of the subfolder of Resources/PhotonPrefabs/Shots that this is from
    /// null or "": defaults to parent gameObject's name
    /// </summary>
    public string subfolderName;
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
    private SpriteRenderer previewSpriteRenderer;
    private Sprite previewSprite;
    private GameObject targetObject;//if not building something new, this is the object to act on

    protected override void Start()
    {
        base.Start();
        if (PV.IsMine)
        {
            if (previewPrefab)
            {
                preview = Instantiate(previewPrefab);
                preview.SetActive(false);
                previewSpriteRenderer = preview.GetComponent<SpriteRenderer>();
                previewSprite = previewSpriteRenderer.sprite;
                previewCollider = preview.GetComponent<Collider2D>();
                previewDisplayer = preview.GetComponent<PreviewDisplayer>();
            }
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
        PreviewDisplayer.PreviewState buildAction = PreviewDisplayer.PreviewState.BUILD;
        if (preview)
        {
            buildAction = getPreviewState();
        }
        if (buildAction == PreviewDisplayer.PreviewState.BUILD)
        {
            if (playerController.ReservedAmina >= minAminaReserved)
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
        }
        else if (buildAction == PreviewDisplayer.PreviewState.UPGRADE)
        {
            float aminaObtained = playerController.collectReservedAmina();
            float aminaMultiplier = aminaObtained / expectedAnimaReserved;
            targetObject.GetComponent<ChargedShotController>().upgradeStats(aminaMultiplier);
        }
        else if (buildAction == PreviewDisplayer.PreviewState.DESTROY)
        {
            PhotonNetwork.Destroy(targetObject);
            playerController.cancelReservedAmina();
        }
        else if (buildAction == PreviewDisplayer.PreviewState.NONE)
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
                Path.Combine("PhotonPrefabs", "Shots", subfolderName, shotPrefabName),
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
        previewSpriteRenderer.sprite = previewSprite;
        GameObject conflictingObject = null;
        bool coHasRB2D = false;
        bool coHasSC = false;
        RaycastHit2D[] rch2ds = new RaycastHit2D[10];
        int count = previewCollider.Cast(Vector2.zero, rch2ds, 0, false);
        for (int i = 0; i < count; i++)
        {
            RaycastHit2D rch2d = rch2ds[i];
            GameObject rchGO = rch2d.collider.gameObject;
            Rigidbody2D rchRB2D = rchGO.GetComponent<Rigidbody2D>();
            ShotController rchSC = rchGO.GetComponent<ShotController>();
            //If the conflicting object is a regular moving shot,
            if (rchRB2D && rchSC)
            {
                //You can build here anyway
                continue;
            }
            //If the conflicting object is non-moving or is not a shot,
            else
            {
                //Double-check to make sure the sprites overlap
                SpriteRenderer coSR = rchGO.GetComponent<SpriteRenderer>();
                bool overlap = coSR.bounds.Intersects(previewSpriteRenderer.bounds);
                if (overlap)
                {
                    //it's conflicting
                    conflictingObject = rchGO;
                    coHasRB2D = rchRB2D;
                    coHasSC = rchSC;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
        if (conflictingObject)
        {
            //If this player owns the conflicting object,
            if (TeamToken.ownedBySamePlayer(gameObject, conflictingObject))
            {
                //if they're the same type,
                if (conflictingObject.name.Contains(shotPrefabName))
                {
                    //upgrade the one there
                    targetObject = conflictingObject;
                    preview.transform.position = conflictingObject.transform.position;
                    return PreviewDisplayer.PreviewState.UPGRADE;
                }
                //else if they're not the same type,
                //but still both constructs
                else if (coHasSC)
                {
                    //delete the object already there
                    targetObject = conflictingObject;
                    preview.transform.position = conflictingObject.transform.position;
                    previewSpriteRenderer.sprite = conflictingObject.GetComponent<ChargedShotController>().previewSprite;
                    return PreviewDisplayer.PreviewState.DESTROY;
                }
            }
            return PreviewDisplayer.PreviewState.NONE;
        }
        else
        {
            targetObject = null;
        }
        if (playerController.ReservedAmina < minAminaReserved)
        {
            return PreviewDisplayer.PreviewState.NONE;
        }
        return PreviewDisplayer.PreviewState.BUILD;
    }
}
