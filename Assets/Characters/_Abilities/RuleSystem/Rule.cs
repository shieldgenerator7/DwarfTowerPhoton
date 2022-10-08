using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rule", menuName = "RuleSystem/Rule")]
public class Rule : ScriptableObject
{
    public AbilitySettings ability;
    public TargetSettings target;
    public RuleTrigger trigger;
    [Multiline(20)]
    public string ruleText;
}
