using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArsenalController : PlayerController
{
    public MultipleCarriedGunController barrageAbility;

    protected override void RegisterDelegates()
    {
        base.RegisterDelegates();
        barrageAbility.onReleaseShots += (shots) =>
        {
            shots.ForEach(shot =>
            {
                MissileController missile = shot.gameObject.FindComponent<MissileController>();
                missile.Target = (Vector2)Utility.MouseWorldPos;
            });
        };
    }
}
