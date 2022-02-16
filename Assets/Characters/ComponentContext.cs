using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores references to other components
/// </summary>
public class ComponentContext : MonoBehaviour
{
    public AminaReloader aminaReloader;//ability called by default when the player runs out of amina
    public PhotonView PV { get; private set; }
    public AminaPool aminaPool { get; private set; }
    public HealthPool healthPool { get; private set; }
    public Damager damager { get; private set; }
    public PlayerInput playerInput { get; private set; }
    public PlayerMovement playerMovement { get; private set; }
    public StatKeeper statKeeper { get; private set; }
    public StatusKeeper statusKeeper { get; private set; }
    public ObjectSpawner objectSpawner { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public StatusAutoEnder statusAutoEnder { get; private set; }
    public TeamToken teamToken { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
    }
    private void InitializeComponents()
    {
        PV = gameObject.FindComponent<PhotonView>();
        playerInput = gameObject.FindComponent<PlayerInput>();
        playerMovement = gameObject.FindComponent<PlayerMovement>();
        playerMovement.Start();
        sr = gameObject.FindComponent<SpriteRenderer>();
        damager = gameObject.FindComponent<Damager>();
        healthPool = gameObject.FindComponent<HealthPool>();
        aminaPool = gameObject.FindComponent<AminaPool>();
        statKeeper = gameObject.FindComponent<StatKeeper>();
        statusKeeper = gameObject.FindComponent<StatusKeeper>();
        objectSpawner = gameObject.FindComponent<ObjectSpawner>();
        statusAutoEnder = gameObject.FindComponent<StatusAutoEnder>();
        statusAutoEnder.Init(statusKeeper);
        teamToken = gameObject.FindComponent<TeamToken>();
    }
}
