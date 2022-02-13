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
    public List<ConditionSet> conditions;

    public override bool Check(RuleContext context)
    {
        switch (combineOption)
        {
            case ConditionSet.CombineOption.AND:
                return conditions.All(cond => cond.Check(context));
            case ConditionSet.CombineOption.OR:
                return conditions.Any(cond => cond.Check(context));
            default:
                throw new System.ArgumentException(
                    $"Unknown combine option: {combineOption}"
                    );
        }
    }
}
