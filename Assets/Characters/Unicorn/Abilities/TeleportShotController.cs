using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportShotController : ShotController
{
    protected override void processCollision(Collider2D collision, bool useInitialDamage)
    {
        //If it is destroyable,
        HealthPool hp = collision.gameObject.FindComponent<HealthPool>();
        if (hp)
        {
            //It doesn't matter what it is, teleport it
            Vector2 teleportPos = Utility.MouseWorldPos;
            Utility.teleportObject(hp.gameObject, teleportPos);
            //and then destroy this shot
            health.Health = 0;
        }
        else if (!collision.isTrigger)
        {
            //Destroy this shot if object is destroyable
            health.Health = 0;
        }
    }
}
