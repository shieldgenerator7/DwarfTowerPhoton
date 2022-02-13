using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionSet", menuName = "Characters/Rule/ConditionSet", order = 0)]
public class ConditionSet : RuleCondition
{
    public enum CombineOption
    {
        AND,
        OR,
    }
    public CombineOption combineOption;
    public List<RuleCondition> conditions;

    public override bool Check(RuleSettings settings, RuleContext context)
    {
        switch (combineOption)
        {
            case ConditionSet.CombineOption.AND:
                return conditions.All(cond => cond.Check(settings, context));
            case ConditionSet.CombineOption.OR:
                return conditions.Any(cond => cond.Check(settings, context));
            default:
                throw new System.ArgumentException(
                    $"Unknown combine option: {combineOption}"
                    );
        }
    }
}
