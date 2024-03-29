﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RainbowPathAbility : PlayerAbility
{
    [Tooltip("The index of the rainbow path in the object spawner")]
    public int rainbowPathIndex;

    public bool active { get; private set; } = false;
    private RainbowPathController rainbowPath;
    private RainbowPathController prevRainbowPath;

    public override void OnButtonDown()
    {
        base.OnButtonDown();

        if (CanActivate)
        {
            activate();
        }
    }
    public override void OnButtonHeld()
    {
        base.OnButtonHeld();
        if (active)
        {
            if (CanStayActive)
            {
                rainbowPath.endPos = playerController.SpawnCenter;
            }
            else
            {
                deactivate();
            }
        }
        else
        {
            if (CanActivate)
            {
                activate();
            }
        }
    }

    public override void OnButtonUp()
    {
        base.OnButtonUp();
        deactivate();
    }

    public override void OnButtonCanceled()
    {
        base.OnButtonCanceled();
        deactivate();
    }

    bool CanActivate
        => rb2d.isMoving()
        && !playerMovement.ForcingMovement
        && aminaPool.requestAminaPerSecond(aminaCost) > 0;

    bool CanStayActive
        => rb2d.isMoving()
        && aminaPool.requestAminaPerSecond(aminaCost) > 0;

    public void activate()
    {
        active = true;
        Vector2 velocity = playerMovement.LastMoveDirection;
        playerMovement.forceMovement(velocity);
        Vector2 spawnPos = playerController.SpawnCenter;
        //Make new rainbow path
        rainbowPath = objectSpawner.spawnObject<RainbowPathController>(
            rainbowPathIndex,
            spawnPos,
            velocity.normalized
            );
        rainbowPath.abilityID = this.abilityID;
        rainbowPath.Start();
        rainbowPath.startPos = spawnPos;
        rainbowPath.endPos = spawnPos;
        rainbowPath.startPos = spawnPos;
    }

    public void deactivate()
    {
        active = false;
        playerMovement.forceMovement(false);
        if (rainbowPath)
        {
            rainbowPath.finish(prevRainbowPath);
            prevRainbowPath = rainbowPath;
            rainbowPath = null;
            prevRainbowPath.onDestroy += (rpc) =>
            {
                if (rpc == prevRainbowPath)
                {
                    prevRainbowPath = null;
                }
                if (rpc == rainbowPath)
                {
                    throw new UnityException("I didn't expect this to happen.");
                }
            };
        }
    }
}
