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
    [Tooltip("How much mana it costs to use this ability, either one-time or per second")]
    public float manaCost;

    /// <summary>
    /// The name of the button that activates this ability
    /// Options: Look in Edit->Project Settings->Input
    /// </summary>
    [Tooltip("The name of the button that activates this ability,\nas listed in Edit->Project Settings->Input")]
    public string buttonName = "Ability1";
    /// <summary>
    /// Whether or not this ability prevents any further abilities from activating
    /// </summary>
    [Tooltip("Should this ability prevent any other abilities from activating while active?")]
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

    public PlayerController playerController { get; private set; }
    public PlayerMovement playerMovement { get; private set; }
    public Rigidbody2D rb2d { get; private set; }
    public PhotonView PV { get; private set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (!playerController)
        {
            playerController = GetComponentInParent<PlayerController>();
        }
        playerMovement = GetComponent<PlayerMovement>();
        if (!playerMovement)
        {
            playerMovement = GetComponentInParent<PlayerMovement>();
        }
        rb2d = GetComponent<Rigidbody2D>();
        if (!rb2d)
        {
            rb2d = GetComponentInParent<Rigidbody2D>();
        }
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

    /// <summary>
    /// Processing that happens even after the button is not pressed until the ability's effect ends
    /// </summary>
    public virtual void OnContinuedProcessing()
    {
        throw new System.NotImplementedException("PlayerAbility.OnContinuedProcessing() is not implemented in subtype " + this.GetType());
    }
}
