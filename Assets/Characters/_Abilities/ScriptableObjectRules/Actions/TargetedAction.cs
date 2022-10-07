using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "TargetedAction", menuName = "Rule/Action/TargetedAction", order = 0)]
public abstract class TargetedAction : RuleAction
{
    public enum Target
    {
        SELF,
        TARGET,
        LAST_CREATED_OBJECT,
    }
    public Target target;

    public sealed override void TakeAction(RuleSettings settings, ref RuleContext context)
    {
        //Find target to take an action on
        ComponentContext targetObj;
        switch (target)
        {
            case Target.SELF:
                targetObj = context.componentContext;
                break;
            case Target.TARGET:
                targetObj = context.target;
                break;
            case Target.LAST_CREATED_OBJECT:
                targetObj = context.lastCreatedObject;
                break;
            default:
                throw new System.ArgumentException($"Unknown target enum value: {target}");
        }
        //Take action
        TakeAction(targetObj, settings, ref context);
    }

    protected abstract void TakeAction(ComponentContext target, RuleSettings settings, ref RuleContext context);

}
