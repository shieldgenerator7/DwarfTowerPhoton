using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveAction", menuName = "Characters/Rule/Action/MoveAction", order = 0)]
public class MoveAction : RuleAction
{
    public enum MoveActionOption
    {
        IN_TARGET_DIR,
        TOWARDS_TARGET_POS,
    }
    public MoveActionOption moveActionOption;
}
