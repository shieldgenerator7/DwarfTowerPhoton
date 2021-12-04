using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private InputState inputState;
    public InputState InputState
    {
        get => inputState;
        private set
        {
            InputState prevInputState = inputState;
            inputState = value;
            if (prevInputState != inputState)
            {
                //TODO: write custom code to determine when a button transitions to down or up
                onInputChanged?.Invoke(inputState);
            }
        }
    }
    public delegate void OnInputChanged(InputState input);
    public event OnInputChanged onInputChanged;


    // Start is called before the first frame update
    void Start()
    {
        if (!this.isPhotonViewMine())
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputState newInput = new InputState();
        newInput.movement = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
            );
        newInput.ability1 = getButtonState("Ability1");
        newInput.ability2 = getButtonState("Ability2");
        newInput.ability3 = getButtonState("Ability3");
        newInput.reload = getButtonState("Reload");
        newInput.moveTowardsCursor = getButtonState("MoveTowardsCursor");
        InputState = newInput;
    }

    ButtonState getButtonState(string buttonName)
    {
        if (Input.GetButtonUp(buttonName))
        {
            return ButtonState.UP;
        }
        else if (Input.GetButton(buttonName))
        {
            if (Input.GetButtonDown(buttonName))
            {
                return ButtonState.DOWN;
            }
            else
            {
                return ButtonState.HELD;
            }
        }
        else
        {
            return ButtonState.NONE;
        }
    }
}
