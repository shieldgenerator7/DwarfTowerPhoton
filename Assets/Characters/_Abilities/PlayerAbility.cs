using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility : MonoBehaviour
{
    //
    // Settings
    //

    /// <summary>
    /// How much mana it costs to use this ability, 
    /// either one-time or per second
    /// </summary>
    public float manaCost;

    /// <summary>
    /// The name of the button that activates this ability
    /// Options: Look in Edit->Project Settings->Input
    /// </summary>
    public string buttonName = "ability1";

    /// <summary>
    /// Whether or not this ability prevents any further abilities from activating
    /// </summary>
    public bool hidesOtherInputs = false;

    //
    // Runtime Variables
    //

    private float buttonHoldStartTime = 0;
    private float buttonHoldEndTime = 0;
    public float ButtonHoldDuration
    {
        get
        {
            if (buttonHoldEndTime > 0)
            {
                return buttonHoldEndTime - buttonHoldStartTime;
            }
            else
            {
                return Time.time - buttonHoldStartTime;
            }
        }
    }

    //
    // Components
    //

    protected PlayerController playerController;
    protected PhotonView PV;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerController = GetComponent<PlayerController>();
        PV = GetComponentInParent<PhotonView>();
    }

    //
    // Input processing
    //

    /// <summary>
    /// Processing that happens when this ability's button is initially pressed
    /// </summary>
    public virtual void OnButtonDown()
    {
        buttonHoldStartTime = Time.time;
        buttonHoldEndTime = 0;
    }

    /// <summary>
    /// Processing that happens when this ability's button is continually pressed
    /// </summary>
    public virtual void OnButtonHeld() { }

    /// <summary>
    /// Processing that happens when this ability's button stops being pressed
    /// </summary>
    public virtual void OnButtonUp()
    {
        buttonHoldEndTime = Time.time;
    }
}
