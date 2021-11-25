using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : PlayerAbility
{
    [Tooltip("Distance to move the player during dash")]
    public float dashDistance = 5;
    [Tooltip("Seconds it takes to complete the dash")]
    public float dashDuration = 0.1f;

    private float dashStartTime = -1;

    protected override void Start()
    {
        base.Start();

        DashAbilityNetwork dan = PV.gameObject.AddComponent<DashAbilityNetwork>();
        dan.dashAbility = this;
    }

    public override void OnButtonDown()
    {
        base.OnButtonDown();
        
        if(aminaPool.requestAmina(manaCost) > 0){
            activate();
        }
    }

    void activate()
    {
        Vector2 moveDir = playerMovement.LastMoveDirection.normalized;
        rb2d.velocity = moveDir * dashDistance / dashDuration;
        dashStartTime = Time.time;
        playerMovement.enabled = false;
        playerController.processAbility(this);
        onDash?.Invoke(true);
        PV.RPC("RPC_OnDash", RpcTarget.Others, true);
    }
    void deactivate()
    {
        if (dashStartTime >= 0)
        {
            dashStartTime = -1;
            rb2d.velocity = Vector2.zero;
            playerMovement.enabled = true;
            playerController.processAbility(this, false);
            onDash?.Invoke(false);
            PV.RPC("RPC_OnDash", RpcTarget.Others, false);
        }
    }
    public delegate void OnDash(bool dashing);
    public event OnDash onDash;

    public void callOnDashDelegate(bool dashing)
    {
        onDash?.Invoke(dashing);
    }

    public override void OnContinuedProcessing()
    {
        if (Time.time >= dashStartTime + dashDuration)
        {
            deactivate();
        }
    }
}
