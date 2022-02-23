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
        RESERVED_AMINA_COST,
    }
    public List<Option> simpleConditions;

    private Timer timer;

    public override bool? CheckAdditional(RuleSettings settings, RuleContext context)
    {
        if (simpleConditions.Count == 0)
        {
            return null;
        }
        ComponentContext compContext = context.componentContext;
        foreach (Option condition in simpleConditions)
        {
            switch (condition)
            {
                case Option.AMINA_COST:
                    bool acceptPartialAmount = settings
                        .Try(RuleSetting.Option.ACCEPT_PARTIAL_AMOUNT)
                        ?? true;
                    return compContext.aminaPool.hasAmina(
                        settings.Get(RuleSetting.Option.AMINA_COST),
                        acceptPartialAmount
                        );
                case Option.AMINA_COST_PER_SECOND:
                    bool acceptPartialAmountPerSecond = settings
                        .Try(RuleSetting.Option.ACCEPT_PARTIAL_AMOUNT)
                        ?? true;
                    return compContext.aminaPool.hasAmina(
                        settings.Get(RuleSetting.Option.AMINA_COST_PER_SECOND) * context.deltaTime,
                        acceptPartialAmountPerSecond
                        );
                case Option.RESERVED_AMINA_COST:
                    float minReservedAmina = settings.Get(RuleSetting.Option.AMINA_COST);
                    return compContext.aminaPool.ReservedAmina >= minReservedAmina;
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
