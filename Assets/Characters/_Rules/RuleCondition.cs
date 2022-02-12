using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RuleCondition", menuName = "Characters/Rule/RuleCondition", order = 0)]
public class RuleCondition: ScriptableObject
{
    public List<EntityType> entityTypes;
    public bool onAlly = true;
    public bool onEnemy = true;
    public bool onTrigger = true;
    public bool onCollision = true; 
}
