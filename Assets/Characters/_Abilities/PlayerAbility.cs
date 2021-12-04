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
    /// How much amina it costs to use this ability, 
    /// either one-time or per second
    /// </summary>
    [Tooltip("How much amina it costs to use this ability, either one-time or per second")]
    public float aminaCost;

    /// <summary>
    /// Which ability slot this ability fits into
    /// </summary>
    [Tooltip("Which ability slot this ability fits into")]
    public AbilitySlot abilitySlot = AbilitySlot.Ability1;
    /// <summary>
    /// Whether or not this ability prevents any further abilities from activating
    /// </summary>
    [Tooltip("Should this ability prevent any other abilities from activating while active?")]
    public bool hidesOtherInputs = false;
    [Tooltip("The unique id per character for this ability, " +
        "as opposed to the other abilities this character has. " +
        "MUST BE UNIQUE per character")]
    public int abilityID;

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
    public ObjectSpawner objectSpawner { get; private set; }
    public Rigidbody2D rb2d { get; private set; }
    public PhotonView PV { get; private set; }
    protected AminaPool aminaPool { get; private set; }
    protected StatKeeper statKeeper { get; private set; }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerController = gameObject.FindComponent<PlayerController>();
        playerMovement = gameObject.FindComponent<PlayerMovement>();
        objectSpawner = gameObject.FindComponent<ObjectSpawner>();
        rb2d = gameObject.FindComponent<Rigidbody2D>();
        PV = gameObject.FindComponent<PhotonView>();
        aminaPool = gameObject.FindComponent<AminaPool>();
        statKeeper = gameObject.FindComponent<StatKeeper>();
        //Modify Ability ID
        abilityID = PV.ViewID * 10 + abilityID;//multiplying by 10 means a possible 10 unique ability IDs per character
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
    /// Processing that happens when this ability's button is pressed when the player is interrupted
    /// </summary>
    public virtual void OnButtonCanceled() { }

    /// <summary>
    /// Processing that happens even after the button is not pressed until the ability's effect ends
    /// </summary>
    public virtual void OnContinuedProcessing()
    {
        throw new System.NotImplementedException($"PlayerAbility.OnContinuedProcessing() is not implemented in subtype {this.GetType()}");
    }
}
