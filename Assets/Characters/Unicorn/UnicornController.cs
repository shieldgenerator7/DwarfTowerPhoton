using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnicornController : PlayerController
{
    [Tooltip("How much amina decays per second while Rainbow Path is not active")]
    public float aminaDecayRate = 5;

    public RainbowPathAbility rainbowPathAbility;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (rainbowPathAbility.active)
        {
        }
        else
        {
            //Amina decay
            aminaPool.drainAmina(aminaDecayRate * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rainbowPathAbility.active)
        {
            Vector2 forceDir = playerMovement.ForceMoveDirection;
            Vector2 normal = collision.contacts[0].normal;
            forceDir = Vector2.Reflect(forceDir, normal);
            playerMovement.forceMovement(forceDir);
            rainbowPathAbility.deactivate();
            rainbowPathAbility.activate();
        }
    }
}
