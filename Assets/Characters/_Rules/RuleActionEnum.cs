



using UnityEngine;

public enum RuleActionEnum
{
    [Tooltip("Move this entity in the target direction.")]
    MOVE_IN_TARGET_DIR,
    [Tooltip("Move this entity towards the target position.")]
    MOVE_TOWARDS_TARGET_POS,

    [Tooltip("Create object (using the SPAWN_INDEX setting and ObjectSpawner component).")]
    CREATE_OBJECT,

    [Tooltip("Deactivate current RuleSet and activate target RuleSet " +
        "(Leave the target RuleSet null to default to the last deactivated RuleSet).")]
    SWITCH_RULESET,//switch out the current ruleset with the target ruleset
    [Tooltip("NotImplemented")]
    ADD_RULESET,
    [Tooltip("NotImplemented")]
    REMOVE_RULESET,

    SET_STAT_MULTIPLIER_FROM_RESERVED_AMINA,
    RESET_STAT_MULTIPLIER,
}
