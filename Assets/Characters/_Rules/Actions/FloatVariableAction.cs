using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VariableAction", menuName = "Rule/Action/VariableAction", order = 0)]
public class FloatVariableAction : RuleAction
{
    public enum Variable
    {
        MULTIPLIER,
        TARGET_DIR,
        TARGET_POS,
    }
    public Variable variable;
    public enum Source
    {
        RESET,
        RESERVED_AMINA,
        CONTROLLER_POS,
        CONTROLLER_LOOK_DIR,
        CURSOR_POS,
    }
    public Source source;

    public override void TakeAction(RuleSettings settings, ref RuleContext context)
    {
        Vector2 sourceVector = Vector2.zero;
        float sourceFloat = 0;
        ComponentContext compContext = context.componentContext;
        switch (source)
        {
            case Source.RESET:
                //TODO: make specific resets for each variable (need separate if statement)
                sourceFloat = 1;
                sourceVector = Vector2.zero;
                break;
            case Source.RESERVED_AMINA:
                float reservedAmina = compContext.aminaPool.ReservedAmina;
                float minAmina = settings.Get(RuleSetting.Option.AMINA_COST);
                float factor = reservedAmina / minAmina;
                float? maxFactor = settings.Try(RuleSetting.Option.MAX_MULTIPLIER);
                if (maxFactor != null)
                {
                    factor = Mathf.Min(factor, maxFactor.Value);
                }
                sourceFloat = factor;
                break;
            case Source.CONTROLLER_POS:
                sourceVector = compContext.teamToken.controller.transform.position;
                break;
            case Source.CONTROLLER_LOOK_DIR:
                sourceVector = compContext.teamToken.controller
                    .gameObject.FindComponent<PlayerController>()?
                    .LookDirection
                    ?? compContext.teamToken.controller.transform.up;
                break;
            case Source.CURSOR_POS:
                sourceVector = Utility.MouseWorldPos;
                break;
            default:
                throw new System.ArgumentException($"Unknown source enum: {source}");
        }
        switch (variable)
        {
            case Variable.MULTIPLIER:
                context.statMultiplier = sourceFloat;
                break;
            case Variable.TARGET_DIR:
                context.targetDir = sourceVector;
                break;
            case Variable.TARGET_POS:
                context.targetPos = sourceVector;
                break;
            default:
                throw new System.ArgumentException($"Unknown variable enum: {variable}");
        }
    }
}
