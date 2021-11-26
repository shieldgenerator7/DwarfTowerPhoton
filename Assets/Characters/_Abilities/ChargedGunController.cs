using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChargedGunController : PlayerAbility
{
    [Tooltip("How much amina is reserved per second of charge")]
    public float aminaReservedPerSecond = 5;//how much amina is reserved per second of charge
    [Tooltip("The amount of anima reserved to get to \"normal\" shot values (NOT a maximum)")]
    public float expectedAnimaReserved = 10;
    [Tooltip("The minimum amount of anima necessary to create the shot")]
    public float minAminaReserved = 5.1f;
    [Tooltip("The index of the charged shot in the object spawner")]
    public int chargedShotIndex;
    [Tooltip("The display-only prefab to spawn while charging the shot")]
    public GameObject previewPrefab;

    private GameObject preview;
    private Collider2D previewCollider;
    private PreviewDisplayer previewDisplayer;
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
                previewCollider = preview.GetComponent<Collider2D>();
                previewDisplayer = preview.GetComponent<PreviewDisplayer>();
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
        aminaPool.reserveAmina(aminaReservedPerSecond * Time.deltaTime);
        //Check to see if the preview collides with anything
        if (preview)
        {
            PreviewDisplayer.PreviewState state = getPreviewState();
            previewDisplayer.updatePreviewColor(state);
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
            if (aminaPool.ReservedAmina >= minAminaReserved)
            {
                Vector2 spawnPos = playerController.SpawnCenter;
                Vector2 dir = ((Vector2)Utility.MouseWorldPos - spawnPos).normalized;
                ChargedShotController chargedShot = objectSpawner.spawnObject<ChargedShotController>(
                    chargedShotIndex,
                    spawnPos,
                    dir
                    );
                float aminaObtained = aminaPool.collectReservedAmina();
                float aminaMultiplier = aminaObtained / expectedAnimaReserved;
                chargedShot.chargeStats(aminaMultiplier);
                onShotFired?.Invoke(
                    chargedShot.gameObject,
                    chargedShot.transform.position,
                    dir
                    );
            }
            else
            {
                aminaPool.cancelReservedAmina();
            }
        }
        else if (buildAction == PreviewDisplayer.PreviewState.UPGRADE)
        {
            float aminaObtained = aminaPool.collectReservedAmina();
            float aminaMultiplier = aminaObtained / expectedAnimaReserved;
            targetObject.GetComponent<ChargedShotController>().upgradeStats(aminaMultiplier);
        }
        else if (buildAction == PreviewDisplayer.PreviewState.DESTROY)
        {
            PhotonNetwork.Destroy(targetObject);
            aminaPool.cancelReservedAmina();
        }
        else if (buildAction == PreviewDisplayer.PreviewState.NONE)
        {
            aminaPool.cancelReservedAmina();
        }
        if (preview)
        {
            preview.SetActive(false);
        }
    }

    public override void OnButtonCanceled()
    {
        base.OnButtonCanceled();

        aminaPool.cancelReservedAmina();

        if (preview)
        {
            preview.SetActive(false);
        }
    }

    /// <summary>
    /// Reacts to a shot being fired from this gun controller
    /// </summary>
    /// <param name="targetPos">The position targetted by this shot</param>
    /// <param name="targetDir">The direction from the player to the target pos, normalized</param>
    public delegate void OnShotFired(GameObject shot, Vector2 targetPos, Vector2 targetDir);
    public event OnShotFired onShotFired;

    /// <summary>
    /// Returns the preview state of the updated preview location
    /// </summary>
    /// <returns></returns>
    private PreviewDisplayer.PreviewState getPreviewState()
    {
        Vector2 playerPos = playerController.SpawnCenter;
        Vector2 targetPos = Utility.MouseWorldPos;
        Vector2 targetDir = (targetPos - playerPos).normalized;
        Vector2 pos = playerPos +
            (targetDir * objectSpawner.objectSpawnInfoList[chargedShotIndex].spawnBuffer);
        return getPreviewState(pos);
    }

    private PreviewDisplayer.PreviewState getPreviewState(Vector2 position)
    {
        preview.transform.position = position;
        previewDisplayer.updatePreviewSprite();
        GameObject conflictingObject = null;
        bool coHasRB2D = false;
        bool coHasSC = false;
        if (previewCollider)
        {
            RaycastHit2D[] rch2ds = new RaycastHit2D[10];
            int count = previewCollider.Cast(Vector2.zero, rch2ds, 0, false);
            for (int i = 0; i < count; i++)
            {
                RaycastHit2D rch2d = rch2ds[i];
                GameObject rchGO = rch2d.collider.gameObject;
                Rigidbody2D rchRB2D = rchGO.GetComponent<Rigidbody2D>();
                ShotController rchSC = rchGO.GetComponent<ShotController>();
                Collider2D coll2d = rchGO.GetComponent<Collider2D>();
                //If the conflicting object is a regular moving shot,
                if (rchRB2D && rchSC)
                {
                    //You can build here anyway
                    continue;
                }
                //If the conflicting object is not solid,
                else if (coll2d.isTrigger)
                {
                    //You can build here anyway
                    continue;
                }
                //If the conflicting object is non-moving or is not a shot,
                else
                {
                    //Double-check to make sure the sprites overlap
                    SpriteRenderer coSR = rchGO.GetComponent<SpriteRenderer>();
                    bool overlap = previewDisplayer.boundsIntersects(coSR.bounds);
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
        }
        if (conflictingObject)
        {
            //If this player owns the conflicting object,
            if (TeamToken.ownedBySamePlayer(gameObject, conflictingObject))
            {
                //if they're the same type,
                if (conflictingObject.name.Contains(
                    objectSpawner.objectSpawnInfoList[chargedShotIndex].objectName
                    ))
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
                    previewDisplayer.updatePreviewSprite(conflictingObject.GetComponent<ChargedShotController>().previewSprite);
                    return PreviewDisplayer.PreviewState.DESTROY;
                }
            }
            return PreviewDisplayer.PreviewState.NONE;
        }
        else
        {
            targetObject = null;
        }
        if (aminaPool.ReservedAmina < minAminaReserved)
        {
            return PreviewDisplayer.PreviewState.NONE;
        }
        return PreviewDisplayer.PreviewState.BUILD;
    }
}
