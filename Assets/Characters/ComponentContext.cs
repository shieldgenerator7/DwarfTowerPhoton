using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
/// <summary>
/// Stores references to other components
/// </summary>
public class ComponentContext : MonoBehaviour
{
    public PhotonView PV { get; private set; }
    public AminaPool aminaPool { get; private set; }
    public HealthPool healthPool { get; private set; }
    public RuleProcessor ruleProcessor { get; private set; }
    public PlayerInput playerInput { get; private set; }
    public PlayerController playerController { get; private set; }
    public PlayerMovement playerMovement { get; private set; }
    public Rigidbody2D rb2d { get; private set; }
    public StatKeeper statKeeper { get; private set; }
    public StatusKeeper statusKeeper { get; private set; }
    public StatusAutoEnder statusAutoEnder { get; private set; }
    public MovementKeeper movementKeeper { get; private set; }
    public ObjectSpawner objectSpawner { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public TeamToken teamToken { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        InitializeComponents();
    }
    public void InitializeComponents()
    {
        PV = gameObject.FindComponent<PhotonView>();
        rb2d = gameObject.FindComponent<Rigidbody2D>();
        playerInput = gameObject.FindComponent<PlayerInput>();
        playerController = gameObject.FindComponent<PlayerController>();
        playerMovement = gameObject.FindComponent<PlayerMovement>();
        playerMovement?.Start();
        sr = gameObject.FindComponent<SpriteRenderer>();
        healthPool = gameObject.FindComponent<HealthPool>();
        aminaPool = gameObject.FindComponent<AminaPool>();
        ruleProcessor = gameObject.FindComponent<RuleProcessor>();
        objectSpawner = gameObject.FindComponent<ObjectSpawner>();
        statKeeper = gameObject.FindComponent<StatKeeper>();
        statusKeeper = gameObject.FindComponent<StatusKeeper>();
        if (statusKeeper)
        {
            statusAutoEnder = gameObject.FindComponent<StatusAutoEnder>();
            statusAutoEnder?.Init(statusKeeper);
        }
        movementKeeper = gameObject.FindComponent<MovementKeeper>();
        teamToken = gameObject.FindComponent<TeamToken>();
    }
}
