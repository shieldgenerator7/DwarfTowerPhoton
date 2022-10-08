



public enum RuleTrigger
{
    //Normal
    OnStart,
    OnUpdate,
    //Health
    OnHealthFull,
    OnHealed,
    OnDamaged,
    OnDied,
    //Collision
    OnHit,
    OnReachTargetPos,
    //Action
    OnDealtHeal,
    OnDealtDamage,
    //Amina
    OnAminaFull,
    OnAminaRecharged,
    OnAminaUsed,
    OnAminaEmpty,
    //State
    OnStatsChanged,
    OnStatusChanged,
    //Input
    OnInput,
    //Controller Input (as in the player who controls this object)
    OnInputController,
}
