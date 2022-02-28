using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiplierAction", menuName = "Rule/Action/MultiplierAction", order = 0)]
public class VariableAction : RuleAction
{
    public enum Action
    {
        SET_STAT_MULTIPLIER_FROM_RESERVED_AMINA,
        RESET_STAT_MULTIPLIER,
    }
    public Action action;

    public override void TakeAction(RuleSettings settings, ref RuleContext context)
    {
        ComponentContext compContext = context.componentContext;
        switch (action)
        {
            case Action.SET_STAT_MULTIPLIER_FROM_RESERVED_AMINA:
                float reservedAmina = compContext.aminaPool.ReservedAmina;
                float minAmina = settings.Get(RuleSetting.Option.AMINA_COST);
                float factor = reservedAmina / minAmina;
                float? maxFactor = settings.Try(RuleSetting.Option.MAX_MULTIPLIER);
                if (maxFactor != null)
                {
                    factor = Mathf.Min(factor, maxFactor.Value);
                }
                context.statMultiplier = factor;
                break;
            case Action.RESET_STAT_MULTIPLIER:
                context.statMultiplier = 1;
                break;
            default:
                throw new System.ArgumentException($"Unknown action enum: {action}");
        }
    }
}
