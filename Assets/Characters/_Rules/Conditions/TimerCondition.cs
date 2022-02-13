using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimerCondition", menuName = "Characters/Rule/TimerCondition", order = 0)]
public class TimerCondition : RuleCondition
{
    private Timer timer;

    public override bool Check(RuleSettings settings, RuleContext context)
    {
        if (timer != null)
        {
            return false;
        }
        else
        {
            timer = TimerManager.StartTimer(
                settings.Get(RuleSetting.Option.ACTIVATE_DELAY),
                () => timer = null
                );
            return true;
        }
    }
}
