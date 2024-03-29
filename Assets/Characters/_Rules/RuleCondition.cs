using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "RuleCondition", menuName = "Rule/Condition/RuleCondition", order = 0)]
public abstract class RuleCondition : ScriptableObject
{
    public abstract bool Check(RuleSettings settings, RuleContext context);
}
