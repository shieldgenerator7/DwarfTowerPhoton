using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OnDieTrigger : RuleTrigger
{
    public float delay = 0;
    public override void RegisterDelegate(Action delegateAction)
    {
        base.RegisterDelegate(delegateAction);

        HealthPool healthPool = null;
        healthPool.onDied += (hp) => delegateAction();
    }
}
