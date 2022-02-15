using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RuleSet", menuName = "Characters/Rule/RuleSet", order = 0)]

public class RuleSet : ScriptableObject
{
    public bool activeAtStart = true;
    public List<Rule> rules;
}
