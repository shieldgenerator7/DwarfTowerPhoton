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
        [Tooltip("[deprecated] Use TimerAction and TimerCondition instead")]
        TIMER,
        RESERVED_AMINA_COST,
        STATIONARY,
        MOVING,
    }
    public List<Option> simpleConditions;

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
                case Option.STATIONARY:
                    return !compContext.rb2d.isMoving();
                case Option.MOVING:
                    return compContext.rb2d.isMoving();
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
