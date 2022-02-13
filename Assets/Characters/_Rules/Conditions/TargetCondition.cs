using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetCondition", menuName = "Characters/Rule/TargetCondition", order = 0)]
public class TargetCondition : RuleCondition
{
    public List<EntityType> entityTypes;
    public bool onAlly = true;
    public bool onEnemy = true;
    public bool onTrigger = true;
    public bool onCollision = true;

    public override bool Check(RuleSettings settings, RuleContext context)
    {
        bool onSameTeam = TeamToken.onSameTeam(context.self, context.target);
        if (onAlly && onSameTeam || onEnemy && !onSameTeam)
        {
            if (onTrigger && context.isTrigger || onCollision && context.isCollision)
            {
                HealthPool hp = context.target.FindComponent<HealthPool>();
                if (hp)
                {
                    if (entityTypes.Contains(hp.entityType))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
