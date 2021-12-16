using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollAbility : PlayerAbility
{
    [Tooltip("How much to increase roll amount by per second while rolling")]
    public float rollPerSecond = 1;
    [Tooltip("The max amount of roll it can achieve, even while rolling")]
    public float maxRoll = 5;

    public float RollPercent => Mathf.Clamp(rollAmount / maxRoll, 0, 1);
    public float RollAmount
    {
        get => rollAmount;
        set
        {
            rollAmount = Mathf.Clamp(value, 0, maxRoll);
            onRollChanged?.Invoke(RollPercent);
        }
    }
    private float rollAmount = 0;
    public delegate void OnRollChanged(float percent);
    public event OnRollChanged onRollChanged;

    public bool Rolling
    {
        get => rolling;
        set
        {
            rolling = value;
            onRollingChanged?.Invoke(rolling);
        }
    }
    private bool rolling = false;
    public delegate void OnRollingChanged(bool rolling);
    public event OnRollingChanged onRollingChanged;


    public override void OnButtonDown()
    {
        base.OnButtonDown();
        if (aminaPool.requestAminaPerSecond(aminaCost) > 0)
        {
            Activate();
        }
    }
    public override void OnButtonHeld()
    {
        base.OnButtonHeld();

        if (aminaPool.requestAminaPerSecond(aminaCost) > 0)
        {
            RollAmount += rollPerSecond * Time.deltaTime;
        }
        else
        {
            Deactivate();
        }
    }
    public override void OnButtonUp()
    {
        base.OnButtonUp();
        Deactivate();
    }
    public override void OnButtonCanceled()
    {
        base.OnButtonCanceled();
        Deactivate();
    }

    private void Activate()
    {
        Rolling = true;
        playerMovement.forceMovement(playerMovement.LastMoveDirection);
    }
    private void Deactivate()
    {
        Rolling = false;
        playerMovement.forceMovement(false);
    }

}
