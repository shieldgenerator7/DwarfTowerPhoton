using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    public SpriteRenderer glow;

    private Artifact _artifact = null;
    public Artifact Artifact
    {
        get => _artifact;
        set
        {
            if (_artifact)
            {
                _artifact.Activate(teamCaptain, false);
                this.carryable.onPickup -= EjectArtifact;
            }
            this.carryable = null;
            _artifact = value;
            if (_artifact)
            {
                _artifact.Activate(teamCaptain, true);
                this.carryable = _artifact.GetComponent<Carryable>();
                this.carryable.onPickup += EjectArtifact;
            }
            glow.enabled = _artifact;
        }
    }

    private TeamTokenCaptain teamCaptain;
    private Carryable carryable;

    // Start is called before the first frame update
    void Start()
    {
        teamCaptain = gameObject.FindComponent<TeamTokenCaptain>();
        Artifact = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Artifact artifact = collision.gameObject.FindComponent<Artifact>();
        if (artifact)
        {
            Artifact = artifact;
            this.carryable.Pickup(null, false);
            artifact.transform.position = transform.position;
        }
    }
    private void EjectArtifact(bool pickup)
    {
        if (pickup)
        {
            Artifact = null;
        }
    }
}
