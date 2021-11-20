using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowPathAbility : PlayerAbility
{
    [Tooltip("The amount of speed players get when walking across the rainbow")]
    public float speedMultiplier = 1.5f;

    public override void OnButtonDown()
    {
        base.OnButtonDown();

        if (playerController.requestAminaPerSecond(manaCost) > 0)
        {
            activate();
        }
    }
    public override void OnButtonHeld()
    {
        base.OnButtonHeld();
        if (playerController.requestAminaPerSecond(manaCost) > 0)
        {
        }
        else
        {
            deactivate();
        }
    }

    public override void OnButtonUp()
    {
        base.OnButtonUp();
        deactivate();
    }

    private void activate()
    {
        playerMovement.forceMovement(rb2d.velocity);
        playerMovement.movementSpeed *= speedMultiplier;
    }

    private void deactivate()
    {
        playerMovement.forceMovement(false);
        playerMovement.movementSpeed /= speedMultiplier;
    }
}
