using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : PlayerAbility
{
    [Tooltip("Distance to move the player during dash")]
    public float dashDistance = 5;
    [Tooltip("Seconds it takes to complete the dash")]
    public float dashDuration = 0.1f;

    private float dashStartTime = -1;

    public override void OnButtonDown()
    {
        base.OnButtonDown();
        
        if(playerController.requestAmina(manaCost) > 0){
            activate();
        }
    }

    void activate()
    {
        Vector2 moveDir = playerMovement.LastMoveDirection.normalized;
        rb2d.velocity = moveDir * dashDistance / dashDuration;
        dashStartTime = Time.time;
        playerMovement.enabled = false;
        playerController.processAbility(this);
    }
    void deactivate()
    {
        if (dashStartTime >= 0)
        {
            dashStartTime = -1;
            rb2d.velocity = Vector2.zero;
            playerMovement.enabled = true;
            playerController.processAbility(this, false);
        }
    }

    public override void OnContinuedProcessing()
    {
        if (Time.time >= dashStartTime + dashDuration)
        {
            deactivate();
        }
    }
}
