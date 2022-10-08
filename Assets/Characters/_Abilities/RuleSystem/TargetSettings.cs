using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Target", menuName = "RuleSystem/Settings/Target")]
public class TargetSettings : RuleSettings
{
    [Header("Allowed Entity Types")]
    public List<EntityType> entityTypes;
    [Header("Allowed Team Alignments")]
    public bool onAlly = true;
    public bool onEnemy = true;
    [Header("Allowed Overlap Types")]
    public bool onTrigger = true;
    public bool onCollision = true;
}
