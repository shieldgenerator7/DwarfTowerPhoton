using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnicornController : PlayerController
{
    public RainbowPathAbility rainbowPathAbility;

    private GameObject lastBounceObject;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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
                redirectPath(transform.position, newforceDir);
            }
        }
    }

    public void redirectPath(Vector2 newPos)
    {
        redirectPath(newPos, playerMovement.ForceMoveDirection);
    }

    public void redirectPath(Vector2 newPos, Vector2 newDir)
    {
        if (rainbowPathAbility.active)
        {
            rainbowPathAbility.deactivate();
            if (newPos != (Vector2)transform.position)
            {
                Utility.teleportObject(gameObject, newPos);
            }
            rainbowPathAbility.activate();
            playerMovement.forceMovement(newDir);
        }
    }

    protected override void onAminaEmpty(float amina)
    {
        base.onAminaEmpty(amina);
        rainbowPathAbility.OnButtonCanceled();
    }
}
