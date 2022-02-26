using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetCondition", menuName = "Rule/Condition/TargetCondition", order = 0)]
public class TargetCondition : RuleCondition
{
    [Header("Allowed Entity Types")]
    public List<EntityType> entityTypes;
    [Header("Allowed Team Alignments")]
    public bool onAlly = true;
    public bool onEnemy = true;
    [Header("Allowed Overlap Types")]
    public bool onTrigger = true;
    public bool onCollision = true;

    public override bool Check(RuleSettings settings, RuleContext context)
    {
        bool onSameTeam = TeamToken.onSameTeam(
            context.componentContext.teamToken,
            context.target.teamToken
            );
        if (onAlly && onSameTeam || onEnemy && !onSameTeam)
        {
            if (onTrigger && context.isTrigger || onCollision && context.isCollision)
            {
                HealthPool hp = context.target.healthPool;
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
