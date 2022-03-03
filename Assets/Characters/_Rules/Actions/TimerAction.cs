using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimerAction", menuName = "Rule/Action/TimerAction", order = 0)]
public class TimerAction : RuleAction
{
    public enum Option
    {
        START,
        CANCEL,
    }
    public Option option;

    public override void TakeAction(RuleSettings settings, ref RuleContext context)
    {
        switch (option)
        {
            case Option.START:
                if (context.timer == null)
                {
                    context.timer = TimerManager.StartTimer(
                        settings.Get(RuleSetting.Option.ACTIVATE_DELAY)
                        );
                }
                else
                {
                    context.timer.reset();
                }
                break;
            case Option.CANCEL:
                context.timer?.cancel();
                context.timer = null;
                break;
            default:
                throw new System.ArgumentException($"Unknown option enum {option}");
        }
       
    }
}
