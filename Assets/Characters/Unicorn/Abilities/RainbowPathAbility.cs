using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RainbowPathAbility : PlayerAbility
{
    [Tooltip("The name of the prefab to spawn from this character's folder under Resources/PhotonPrefabs/Shots")]
    public string rainbowPathPrefabName;
    /// <summary>
    /// The name of the subfolder of Resources/PhotonPrefabs/Shots that this is from
    /// null or "": defaults to parent gameObject's name
    /// </summary>
    [Tooltip("The name of this character. Leave blank to default to parent GameObject's name")]
    public string subfolderName;

    private Vector2 PavePosition => (Vector2)transform.position + (Vector2.up * 0.5f);

    private bool active = false;
    private RainbowPathController rainbowPath;

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

        if (rb2d.isMoving())
        {
            if (playerController.requestAminaPerSecond(manaCost) > 0)
            {
                activate();
            }
        }
    }
    public override void OnButtonHeld()
    {
        base.OnButtonHeld();
        if (active)
        {
            if (rb2d.isMoving()
                && playerController.requestAminaPerSecond(manaCost) > 0
                )
            {
                rainbowPath.endPos = PavePosition;
            }
            else
            {
                deactivate();
            }
        }
        else
        {
            if (rb2d.isMoving())
            {
                if (playerController.requestAminaPerSecond(manaCost) > 0)
                {
                    activate();
                }
            }
        }
    }

    public override void OnButtonUp()
    {
        base.OnButtonUp();
        deactivate();
    }

    private void activate()
    {
        active = true;
        playerMovement.forceMovement(playerMovement.LastMoveDirection);
        //Make new rainbow path
        rainbowPath = pavePath();
        rainbowPath.startPos = PavePosition;
        rainbowPath.endPos = PavePosition;
        rainbowPath.startPos = PavePosition;
    }

    private void deactivate()
    {
        active = false;
        playerMovement.forceMovement(false);
        rainbowPath = null;
    }


    /// <summary>
    /// Paves a path
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="targetPos"></param>
    public RainbowPathController pavePath()
    {
        if (PV.IsMine)
        {
            Vector2 playerPos = transform.position;
            GameObject path = PhotonNetwork.Instantiate(
                Path.Combine("PhotonPrefabs", "Shots", subfolderName, rainbowPathPrefabName),
                playerPos,
                Quaternion.identity
                );

            //OnPathPaved Delegate
            onPathPaved?.Invoke(path, playerPos, playerPos);
            //Return
            return path.GetComponent<RainbowPathController>();
        }
        return null;
    }
    /// <summary>
    /// Reacts to a path being paved
    /// </summary>
    /// <param name="targetPos">The position targetted by this shot</param>
    public delegate void OnPathPaved(GameObject path, Vector2 targetPos, Vector2 targetDir);
    public OnPathPaved onPathPaved;
}
