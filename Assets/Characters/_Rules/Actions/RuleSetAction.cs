using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RuleSetAction", menuName = "Rule/Action/RuleSetAction", order = 0)]
public class RuleSetAction : TargetedAction
{
    public enum Action
    {
        [Tooltip("Deactivate current RuleSet and activate target RuleSet " +
        "(Leave the target RuleSet null to default to the last deactivated RuleSet).")]
        SWITCH_RULESET,//switch out the current ruleset with the target ruleset
        [Tooltip("Add the target RuleSet to the list of active rulesets")]
        ADD_RULESET,
        [Tooltip("Remove the target RuleSet from the list of active rulesets " +
            "(Leave the target RuleSet null to default to removing this RuleSet).")]
        REMOVE_RULESET,
    }
    public Action action;

    protected override void TakeAction(ComponentContext target, RuleSettings settings, ref RuleContext context)
    {
        switch (action)
        {
            case Action.SWITCH_RULESET:
                RuleSet currentRuleSet = context.currentRuleSet;
                RuleSet targetRuleSet = settings.targetRuleSet
                    ?? context.lastDeactivatedRuleSet;
                context.ruleSetActions.Add(
                    currentRuleSet,
                    RuleContext.RuleSetAction.DEACTIVATE
                    );
                context.ruleSetActions.Add(
                    targetRuleSet,
                    RuleContext.RuleSetAction.ACTIVATE
                    );
                break;
            case Action.ADD_RULESET:
                RuleSet addRuleSet = settings.targetRuleSet;
                context.ruleSetActions.Add(
                    addRuleSet,
                    RuleContext.RuleSetAction.ADD
                    );
                break;
            case Action.REMOVE_RULESET:
                RuleSet removeRuleSet = settings.targetRuleSet
                    ?? context.currentRuleSet;
                context.ruleSetActions.Add(
                    removeRuleSet,
                    RuleContext.RuleSetAction.REMOVE
                    );
                break;
        }
    }
}
