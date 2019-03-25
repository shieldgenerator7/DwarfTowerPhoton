using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityContextSwitcher : PlayerAbility
{
    public AbilityContext abilityContext;

    public override void OnButtonUp()
    {
        base.OnButtonUp();
        playerController.AbilityContext = abilityContext;
    }
}
