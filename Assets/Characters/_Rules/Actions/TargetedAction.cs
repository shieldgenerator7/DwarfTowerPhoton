using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetedAction", menuName = "Rule/Action/TargetedAction", order = 0)]
public class TargetedAction : RuleAction
{
    public enum Target
    {
        SELF,
        TARGET,
        LAST_CREATED_OBJECT,
    }
    public Target target;

    public enum Action
    {
        DAMAGE,
        HEAL,
        ADD_STAT_LAYER,
        REMOVE_STAT_LAYER,
        ADD_STATUS_LAYER,
        REMOVE_STATUS_LAYER,
    }
    public Action action;

    public override void TakeAction(RuleSettings settings, ref RuleContext context)
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
        //Initialize variables
        ComponentContext compContext = context.componentContext;
        StatLayer stats = compContext.statKeeper?.selfStats.Stats ?? new StatLayer(-1);
        int abilityID = settings.AbilityID(compContext.PV.ViewID);
        //Take the selected action
        switch (action)
        {
            case Action.DAMAGE:
                targetObj.healthPool.Health += -stats.damage * context.deltaTime;
                break;
            case Action.HEAL:
                targetObj.healthPool.Health += stats.damage * context.deltaTime;
                break;
            case Action.ADD_STAT_LAYER:
                StatLayer multiplierLayer = settings.statLayer.Multiply(context.statMultiplier);
                targetObj.statKeeper.selfStats.addLayer(abilityID, multiplierLayer);
                break;
            case Action.REMOVE_STAT_LAYER:
                targetObj.statKeeper.selfStats.removeLayer(abilityID);
                break;
            case Action.ADD_STATUS_LAYER:
                StatusLayer statusLayer = new StatusLayer(settings.statusLayer.StatusEffects);
                targetObj.statusKeeper.addLayer(abilityID, statusLayer);
                break;
            case Action.REMOVE_STATUS_LAYER:
                targetObj.statusKeeper.removeLayer(abilityID);
                break;
            default:
                throw new System.ArgumentException($"Unknown action enum value: {action}");
        }
    }
}
