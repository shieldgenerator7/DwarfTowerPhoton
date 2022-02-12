using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Rule", menuName = "Characters/Rule/Rule", order = 0)]
public class Rule : ScriptableObject
{
    public RuleTrigger trigger;
    public List<RuleCondition> conditions;
    public List<RuleAction> actions;
}
