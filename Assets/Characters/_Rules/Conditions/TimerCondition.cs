using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimerCondition", menuName = "Characters/Rule/TimerCondition", order = 0)]
public class TimerCondition : RuleCondition
{
    public float trueDelay = 5;

    private Timer timer;

    public override bool Check(RuleContext context)
    {
        if (timer != null)
        {
            return false;
        }
        else
        {
            timer = TimerManager.StartTimer(trueDelay);
            timer.onTimerCompleted += () => timer = null;
            return true;
        }
    }
}
