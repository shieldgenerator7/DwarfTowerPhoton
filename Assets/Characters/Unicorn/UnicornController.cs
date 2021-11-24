using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnicornController : PlayerController
{
    [Tooltip("How much amina decays per second while Rainbow Path is not active")]
    public float aminaDecayRate = 5;

    public RainbowPathAbility rainbowPathAbility;

    private GameObject lastBounceObject;

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
        lastBounceObject = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rainbowPathAbility.active)
        {
            if (collision.gameObject != lastBounceObject)
            {
                lastBounceObject = collision.gameObject;
                Vector2 forceDir = playerMovement.ForceMoveDirection;
                Vector2 normal = collision.contacts[0].normal;
                Vector2 newforceDir = Vector2.Reflect(forceDir, normal);
                rainbowPathAbility.deactivate();
                rainbowPathAbility.activate();
                playerMovement.forceMovement(newforceDir);
            }
        }
    }
}
