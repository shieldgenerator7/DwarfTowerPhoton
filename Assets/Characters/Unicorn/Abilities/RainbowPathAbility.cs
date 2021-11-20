using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowPathAbility : PlayerAbility
{
    public float dashDistance = 5;

    private bool zeroVelocity = false;

    public override void OnButtonDown()
    {
        base.OnButtonDown();
        
        if(playerController.requestAmina(manaCost) > 0){
            Vector2 moveDir = (Utility.MouseWorldPos - transform.position).normalized;
            //Give enough force for rb2d to move character entire distance in single frame
            rb2d.velocity = moveDir * dashDistance / Time.deltaTime;
            zeroVelocity = true;
        }
    }
    public override void OnButtonHeld()
    {
        base.OnButtonHeld();
        if (zeroVelocity)
        {
            zeroVelocity = false;
            rb2d.velocity = Vector2.zero;
        }
    }
}
