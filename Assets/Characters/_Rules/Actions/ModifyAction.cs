using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifyAction", menuName = "Rule/Action/ModifyAction", order = 0)]
public class ModifyAction : TargetedAction
{
    public enum Action
    {
        ADD_STAT_LAYER,
        REMOVE_STAT_LAYER,
        ADD_STATUS_LAYER,
        REMOVE_STATUS_LAYER,
    }
    public Action action;

    protected override void TakeAction(ComponentContext target, RuleSettings settings, ref RuleContext context)
    {
        //Initialize variables
        ComponentContext compContext = context.componentContext;
        int abilityID = settings.AbilityID(compContext.PV.ViewID);
        //Take the selected action
        switch (action)
        {
            case Action.ADD_STAT_LAYER:
                StatLayer multiplierLayer = settings.statLayer.Multiply(context.statMultiplier);
                target.statKeeper.selfStats.addLayer(abilityID, multiplierLayer);
                break;
            case Action.REMOVE_STAT_LAYER:
                target.statKeeper.selfStats.removeLayer(abilityID);
                break;
            case Action.ADD_STATUS_LAYER:
                StatusLayer statusLayer = new StatusLayer(settings.statusLayer.StatusEffects);
                target.statusKeeper.addLayer(abilityID, statusLayer);
                break;
            case Action.REMOVE_STATUS_LAYER:
                target.statusKeeper.removeLayer(abilityID);
                break;
            default:
                throw new System.ArgumentException($"Unknown action enum value: {action}");
        }
    }
}
