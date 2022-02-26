



using UnityEngine;

public enum RuleActionEnum
{
    [Tooltip("Move this entity in the target direction.")]
    MOVE_IN_TARGET_DIR,
    [Tooltip("Move this entity towards the target position.")]
    MOVE_TOWARDS_TARGET_POS,

    [Tooltip("Create object (using the SPAWN_INDEX setting and ObjectSpawner component).")]
    CREATE_OBJECT,
}
