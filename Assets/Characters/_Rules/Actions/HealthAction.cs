using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthAction", menuName = "Rule/Action/HealthAction", order = 0)]
public class HealthAction : TargetedAction
{
    public enum Action
    {
        DAMAGE,
        DAMAGE_PER_SECOND,
        HEAL,
        HEAL_PER_SECOND,
    }
    public Action action;

    protected override void TakeAction(ComponentContext target, RuleSettings settings, ref RuleContext context)
    {
        //Initialize variables
        ComponentContext compContext = context.componentContext;
        StatLayer stats = compContext.statKeeper?.selfStats.Stats ?? new StatLayer(-1);
        float damage = stats.damage;
        //Take the selected action
        switch (action)
        {
            case Action.DAMAGE:
                target.healthPool.Health += -damage;
                break;
            case Action.DAMAGE_PER_SECOND:
                target.healthPool.Health += -damage * context.deltaTime;
                break;
            case Action.HEAL:
                target.healthPool.Health += damage;
                break;
            case Action.HEAL_PER_SECOND:
                target.healthPool.Health += damage * context.deltaTime;
                break;
        }
    }
}
