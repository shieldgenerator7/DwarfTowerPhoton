



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
    //Input
    OnInputChanged,
    //State
    OnStatsChanged,
    OnStatusChanged,
    //Controller Input
    //TODO: reorder these so they're grouped better
    OnControllerInputChanged,
}
