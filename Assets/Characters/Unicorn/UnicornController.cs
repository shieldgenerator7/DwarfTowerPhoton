using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnicornController : PlayerController
{
    [Tooltip("How much amina decays per second while Rainbow Path is not active")]
    public float aminaDecayRate = 5;

    public RainbowPathAbility rainbowPathAbility;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (rainbowPathAbility.active)
        {
        }
        else
        {
            //Amina decay
            Amina -= aminaDecayRate * Time.deltaTime;
        }
    }
}
