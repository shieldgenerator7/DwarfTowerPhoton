using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariableAction", menuName = "Rule/Action/FloatVariableAction", order = 0)]
public class FloatVariableAction : VariableAction
{
    public enum Variable
    {
        MULTIPLIER,
    }
    public Variable variable;
    public enum Source
    {
        RESET,
        RESERVED_AMINA,
    }
    public Source source;

    protected override object GetSource(RuleSettings settings, RuleContext context)
    {
        ComponentContext compContext = context.componentContext;
        switch (source)
        {
            case Source.RESET:
                //TODO: make specific resets for each variable (need separate if statement)
                return 1.0f;
            case Source.RESERVED_AMINA:
                float reservedAmina = compContext.aminaPool.ReservedAmina;
                float minAmina = settings.Get(RuleSetting.Option.AMINA_COST);
                float factor = reservedAmina / minAmina;
                float? maxFactor = settings.Try(RuleSetting.Option.MAX_MULTIPLIER);
                if (maxFactor != null)
                {
                    factor = Mathf.Min(factor, maxFactor.Value);
                }
                return factor;
            default:
                throw new System.ArgumentException($"Unknown source enum: {source}");
        }
    }

    protected override void SetVariable(object source, RuleSettings settings, ref RuleContext context)
    {
        float sourceFloat = (float)source;
        switch (variable)
        {
            case Variable.MULTIPLIER:
                context.statMultiplier = sourceFloat;
                break;
            default:
                throw new System.ArgumentException($"Unknown variable enum: {variable}");
        }
    }
}
