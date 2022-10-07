using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimerCondition", menuName = "Rule/Condition/TimerCondition", order = 0)]
public class TimerCondition : RuleCondition
{
    public bool onValue;

    public override bool Check(RuleSettings settings, RuleContext context)
    {
        return onValue == (context.timer != null && !context.timer.Completed);
    }
}
