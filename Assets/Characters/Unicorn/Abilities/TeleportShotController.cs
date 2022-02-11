using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportShotController : ShotController
{

    protected override void Start()
    {
        base.Start();
        teamToken.onControllerChanged += (controller) => updateSpeed();
        updateSpeed();
    }

    private void updateSpeed()
    {
        if (teamToken.controller != teamToken)
        {
            rb2d.velocity = rb2d.velocity.normalized
                * teamToken.controller.gameObject.FindComponent<Rigidbody2D>()
                .velocity.magnitude;
        }
    }

    protected override void processCollision(Collider2D collision, bool useInitialDamage)
    {
        //If it's a flag, don't teleport it
        Rigidbody2D rb2d = collision.gameObject.FindComponent<Rigidbody2D>();
        if (rb2d && rb2d.gameObject.CompareTag("TeamFlag"))
        {
            return;
        }
        //If it is destroyable,
        HealthPool hp = collision.gameObject.FindComponent<HealthPool>();
        if (hp)
        {
            //It doesn't matter what it is, teleport it
            Vector2 teleportPos = Utility.MouseWorldPos;
            //Check if it's a Unicorn using Rainbow Path
            UnicornController unicorn = collision.gameObject.FindComponent<UnicornController>();
            if (unicorn)
            {
                unicorn.redirectPath(teleportPos);
            }
            //Teleport like usual
            Utility.teleportObject(hp.gameObject, teleportPos);
            //If it's a WeaponController (like the weapons from Astral)
            WeaponController weapon = collision.gameObject.FindComponent<WeaponController>();
            if (weapon)
            {
                //Make it unowned
                weapon.teamToken.switchController(null);
            }
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
