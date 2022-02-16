using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SimpleCondition", menuName = "Rule/Condition/SimpleCondition", order = 0)]
public class SimpleConditionSet : ConditionSet
{
    public enum Option
    {
        AMINA_COST,
        AMINA_COST_PER_SECOND,
        TIMER,
    }
    public List<Option> simpleConditions;

    private Timer timer;

    public override bool? CheckAdditional(RuleSettings settings, RuleContext context)
    {
        if (simpleConditions.Count == 0)
        {
            return null;
        }
        foreach (Option condition in simpleConditions)
        {
            switch (condition)
            {
                case Option.AMINA_COST:
                    {
                        AminaPool aminaPool = context.self.FindComponent<AminaPool>();
                        bool acceptPartialAmount = settings
                            .Try(RuleSetting.Option.ACCEPT_PARTIAL_AMOUNT)
                            ?? true;
                        return aminaPool.hasAmina(
                            settings.Get(RuleSetting.Option.AMINA_COST),
                            acceptPartialAmount
                            );
                    }
                case Option.AMINA_COST_PER_SECOND:
                    {
                        AminaPool aminaPool = context.self.FindComponent<AminaPool>();
                        bool acceptPartialAmount = settings
                            .Try(RuleSetting.Option.ACCEPT_PARTIAL_AMOUNT)
                            ?? true;
                        return aminaPool.hasAmina(
                            settings.Get(RuleSetting.Option.AMINA_COST_PER_SECOND) * context.deltaTime,
                            acceptPartialAmount
                            );
                    }
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
        throw new System.ArgumentException(
            $"SimpleConditionSet.CheckAdditional(): " +
            $"this statement should not be reachable! " +
            $"simpleConditions.Count: {simpleConditions.Count}");
    }
}
