using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    private Artifact _artifact;
    public Artifact Artifact
    {
        get => _artifact;
        set
        {
            this.carryable = null;
            if (_artifact)
            {
                _artifact.Activate(teamCaptain, false);
                this.carryable.onPickup -= EjectArtifact;
            }
            _artifact = value;
            if (_artifact)
            {
                _artifact.Activate(teamCaptain, true);
                this.carryable = _artifact.GetComponent<Carryable>();
                this.carryable.onPickup += EjectArtifact;
            }
        }
    }

    private TeamTokenCaptain teamCaptain;
    private Carryable carryable;

    // Start is called before the first frame update
    void Start()
    {
        teamCaptain = gameObject.FindComponent<TeamTokenCaptain>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Artifact artifact = collision.gameObject.FindComponent<Artifact>();
        if (artifact)
        {
            Carryable carryable = artifact.GetComponent<Carryable>();
            carryable.Pickup(null, false);
            Artifact = artifact;
            artifact.transform.position = transform.position;
        }
    }
    private void EjectArtifact(bool pickup)
    {
        Artifact = null;
    }
}
