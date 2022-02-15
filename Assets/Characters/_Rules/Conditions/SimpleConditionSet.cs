using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SimpleCondition", menuName = "Characters/Rule/SimpleCondition", order = 0)]
public class SimpleConditionSet : ConditionSet
{
    public enum Option
    {
        AMINA_COST,
        TIMER,
    }
    public List<Option> simpleConditions;

    private Timer timer;

    public override bool Check(RuleSettings settings, RuleContext context)
    {
        if (simpleConditions.Count == 0)
        {
            return true;
        }
        foreach (Option condition in simpleConditions)
        {
            switch (condition)
            {
                case Option.AMINA_COST:
                    AminaPool aminaPool = context.self.FindComponent<AminaPool>();
                    bool acceptPartialAmount = settings
                        .Try(RuleSetting.Option.ACCEPT_PARTIAL_AMOUNT)
                        ?? true;
                    return aminaPool.hasAmina(
                        settings.Get(RuleSetting.Option.AMINA_COST) * context.deltaTime,
                        acceptPartialAmount
                        );
                case Option.TIMER:
                    if (timer == null)
                    {
                        timer = TimerManager.StartTimer(
                            settings.Get(RuleSetting.Option.ACTIVATE_DELAY),
                            () => timer = null
                            );
                        return true;
                    }
                    return false;
                default:
                    throw new System.ArgumentException($"Condition not recognized: {condition}");
            }
        }
        return false;
    }
}
