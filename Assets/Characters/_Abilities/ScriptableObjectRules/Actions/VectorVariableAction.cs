using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VectorVariableAction", menuName = "Rule/Action/VectorVariableAction", order = 0)]
public class VectorVariableAction : VariableAction
{
    public enum Variable
    {
        TARGET_DIR,
        TARGET_POS,
    }
    public Variable variable;
    public enum Source
    {
        RESET,
        CONTROLLER_POS,
        CONTROLLER_LOOK_DIR,
        CURSOR_POS,
    }
    public Source source;

    protected override object GetSource(RuleSettings settings, RuleContext context)
    {
        ComponentContext compContext = context.componentContext;
        switch (source)
        {
            case Source.RESET:
                //TODO: make specific resets for each variable (need separate if statement)
                return Vector2.zero;
            case Source.CONTROLLER_POS:
                return compContext.teamToken.controller.transform.position;
            case Source.CONTROLLER_LOOK_DIR:
                return compContext.teamToken.controller
                    .gameObject.FindComponent<PlayerController>()?
                    .LookDirection
                    ?? compContext.teamToken.controller.transform.up;
            case Source.CURSOR_POS:
                return Utility.MouseWorldPos;
            default:
                throw new System.ArgumentException($"Unknown source enum: {source}");
        }
    }

    protected override void SetVariable(object source, RuleSettings settings, ref RuleContext context)
    {
        Vector2 sourceVector = (Vector2)source;
        switch (variable)
        {
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
