using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Rule", menuName = "Characters/Rule", order = 0)]
public class Rule : ScriptableObject
{
    public List<RuleTrigger> triggers;
    public List<RuleCondition> conditions;
    public List<RuleAction> actions;
    public RuleTrigger otestdsf;

    public void Init()
    {
        triggers.ForEach(trigger => trigger.RegisterDelegate(Process));
    }

    public void Process()
    {
        bool takeAction = conditions.Any(condition => condition.Check());
        if (takeAction)
        {
            actions.ForEach(action => action.Act());
        }
    }
}
