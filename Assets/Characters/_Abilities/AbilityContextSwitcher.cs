﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityContextSwitcher : PlayerAbility
{
    [Tooltip("The ability context to switch to when activated")]
    public AbilityContext abilityContext;

    [Tooltip("The abilities that can activate this switcher")]
    public List<PlayerAbility> switchOnAbilities;

    protected override void Start()
    {
        base.Start();
        foreach(PlayerAbility pa in switchOnAbilities)
        {
            if (pa is GunController)
            {
                ((GunController)pa).onShotFired += switchContextOnShot;
            }
            if (pa is ChargedGunController)
            {
                ((ChargedGunController)pa).onShotFired += switchContextOnShot;
            }
        }
    }

    public override void OnButtonUp()
    {
        base.OnButtonUp();
        switchContext();
    }

    public void switchContext()
    {
        playerController.AbilityContext = abilityContext;
    }

    public void switchContextOnShot(GameObject shot, Vector2 pos, Vector2 dir)
    {
        switchContext();
    }
}
