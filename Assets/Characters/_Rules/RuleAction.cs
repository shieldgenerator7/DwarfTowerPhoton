



using UnityEngine;

public enum RuleAction
{
    [Tooltip("Move this entity in the target direction.")]
    MOVE_IN_TARGET_DIR,
    [Tooltip("Move this entity towards the target position.")]
    MOVE_TOWARDS_TARGET_POS,

    [Tooltip("Damage the target entity.")]
    DAMAGE,
    [Tooltip("Damage this entity.")]
    DAMAGE_SELF,

    [Tooltip("Create object (using the SPAWN_INDEX setting and ObjectSpawner component).")]
    CREATE_OBJECT,

    [Tooltip("Deactivate current RuleSet and activate target RuleSet " +
        "(Leave the target RuleSet null to default to the last deactivated RuleSet).")]
    SWITCH_RULESET,//switch out the current ruleset with the target ruleset
    [Tooltip("NotImplemented")]
    ADD_RULESET,
    [Tooltip("NotImplemented")]
    REMOVE_RULESET,

    [Tooltip("Use amina (setting: AMINA_COST).")]
    USE_AMINA,
    [Tooltip("Use amina over time (setting: AMINA_COST_PER_SECOND).")]
    USE_AMINA_PER_SECOND,
    [Tooltip("Recharge amina (setting: AMINA_COST).")]
    RECHARGE_AMINA,
    [Tooltip("Recharge amina over time (setting: AMINA_COST_PER_SECOND).")]
    RECHARGE_AMINA_PER_SECOND,
    RESERVE_AMINA_PER_SECOND,
    USE_RESERVED_AMINA,
    CANCEL_RESERVED_AMINA,

    SET_STAT_MULTIPLIER_FROM_RESERVED_AMINA,
    RESET_STAT_MULTIPLIER,

    ADD_STAT_LAYER_TO_SELF,
    ADD_STAT_LAYER_TO_TARGET,
    ADD_STAT_LAYER_TO_CREATED_OBJECT,
    REMOVE_STAT_LAYER_FROM_SELF,
    REMOVE_STAT_LAYER_FROM_TARGET,
    REMOVE_STAT_LAYER_FROM_CREATED_OBJECT,
    
    ADD_STATUS_LAYER_TO_SELF,
    ADD_STATUS_LAYER_TO_TARGET,
    ADD_STATUS_LAYER_TO_CREATED_OBJECT,
    REMOVE_STATUS_LAYER_FROM_SELF,
    REMOVE_STATUS_LAYER_FROM_TARGET,
    REMOVE_STATUS_LAYER_FROM_CREATED_OBJECT,
}
