using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rule", menuName = "RuleSystem/Rule")]
public class Rule : ScriptableObject
{
    public AbilitySettings ability;
    public RuleTrigger trigger;
    [Multiline(10)]
    public string ruleText;
}
