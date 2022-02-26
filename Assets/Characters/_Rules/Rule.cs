using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Rule", menuName = "Rule/Rule", order = 0)]
public class Rule : ScriptableObject
{
    public RuleSettings settings;
    public RuleTrigger trigger;
    public RuleCondition condition;
    public List<RuleActionEnum> actionEnums;
}
