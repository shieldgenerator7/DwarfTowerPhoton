using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowPathAbility : PlayerAbility
{
    [Tooltip("The amount of speed players get when walking across the rainbow")]
    public float speedMultiplier = 1.5f;
    [Tooltip("The name of the prefab to spawn from this character's folder under Resources/PhotonPrefabs/Shots")]
    public string pathPrefabName;
    /// <summary>
    /// The name of the subfolder of Resources/PhotonPrefabs/Shots that this is from
    /// null or "": defaults to parent gameObject's name
    /// </summary>
    [Tooltip("The name of this character. Leave blank to default to parent GameObject's name")]
    public string subfolderName;
    [Tooltip("How far away from the player the path spawns")]
    public float spawnBuffer = 1;//how far away from the player the path spawns

    private bool active = false;

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
        playerMovement.movementSpeed *= speedMultiplier;
    }

    private void deactivate()
    {
        active = false;
        playerMovement.forceMovement(false);
        playerMovement.movementSpeed /= speedMultiplier;
    }
}
