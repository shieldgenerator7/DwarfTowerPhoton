using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonCondition", menuName = "Characters/Rule/ButtonCondition", order = 0)]
public class ButtonCondition : RuleCondition
{
    public AbilitySlot inputButton;
    public ButtonState buttonState;

    public override bool Check(RuleSettings settings, RuleContext context)
        => context.inputState.Button(inputButton) == buttonState;
}
