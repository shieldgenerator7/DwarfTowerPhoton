using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RainbowPathAbility : PlayerAbility
{
    [Tooltip("The index of the rainbow path in the object spawner")]
    public int rainbowPathIndex;

    private Vector2 PavePosition => (Vector2)transform.position + (Vector2.up * 0.5f);

    private bool active = false;
    private RainbowPathController rainbowPath;

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
        Vector2 velocity = playerMovement.LastMoveDirection;
        playerMovement.forceMovement(velocity);
        //Make new rainbow path
        rainbowPath = objectSpawner.spawnObject<RainbowPathController>(
            rainbowPathIndex,
            transform.position,
            velocity.normalized
            );
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
}
