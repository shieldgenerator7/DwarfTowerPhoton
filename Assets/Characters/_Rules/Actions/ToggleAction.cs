using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToggleAction", menuName = "Rule/Action/ToggleAction", order = 0)]
public class ToggleAction : TargetedAction
{
    public enum Option
    {
        MOVE_INPUT_ENABLED,
        MOVEMENT_ALLOWED,
    }
    public Option option;

    public bool boolValue = true;

    protected override void TakeAction(ComponentContext target, RuleSettings settings, ref RuleContext context)
    {
        switch (option)
        {
            //case Option.MOVE_INPUT_ENABLED:
            //    break;
            case Option.MOVEMENT_ALLOWED:
                target.movementKeeper.MovementAllowed = boolValue;
                break;
            default:
                throw new System.ArgumentException($"Unknown option enum: {option}");
        }
    }
}
