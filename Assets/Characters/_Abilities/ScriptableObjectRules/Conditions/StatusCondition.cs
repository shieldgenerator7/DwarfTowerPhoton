using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusCondition", menuName = "Rule/Condition/StatusCondition", order = 0)]
public class StatusCondition : RuleCondition
{
    public enum CombineOption
    {
        ALL,
        ANY,
        NONE,
    }
    public CombineOption requiredOption;

    public StatusLayer status;

    public override bool Check(RuleSettings settings, RuleContext context)
    {
        StatusLayer currentStatus = context.componentContext.statusKeeper.Status;
        switch (requiredOption)
        {
            case CombineOption.ALL:
                return status.StatusEffects.All(
                    effect => currentStatus.Has(effect)
                    );
            case CombineOption.ANY:
                return status.StatusEffects.Any(
                    effect => currentStatus.Has(effect)
                    );
            case CombineOption.NONE:
                return !status.StatusEffects.Any(
                    effect => currentStatus.Has(effect)
                    );
            default:
                throw new System.ArgumentException($"Unknown CombineOption: {requiredOption}");
        }
    }
}
