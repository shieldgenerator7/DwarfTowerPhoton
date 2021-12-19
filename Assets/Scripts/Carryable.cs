using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryable : MonoBehaviour
{
    public float carryBuffer = 0.5f;

    private Vector2 holdDirection = Vector2.zero;

    private PlayerMovement carrier;
    private PhotonView PV;
    private CaravanController caravan;

    // Start is called before the first frame update
    void Start()
    {
        PV = gameObject.FindComponent<PhotonView>();
        caravan = FindObjectOfType<CaravanController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (carrier && (PV.IsMine || carrier.isPhotonViewMine()))
        {
            transform.position = (Vector2)carrier.transform.position
                + (holdDirection * carryBuffer);
        }
    }

    public void Pickup(PlayerMovement carrier, bool carry)
    {
        if (carry)
        {
            this.carrier = carrier;
            //Hold direction
            holdDirection = TeamToken.getTeamToken(carrier.gameObject)
                .teamCaptain.transform.position
                - caravan.transform.position;
            holdDirection.x = 0;
            holdDirection.Normalize();
        }
        else
        {
            this.carrier = null;
        }
        onPickup?.Invoke(carry);
    }
    public delegate void OnPickup(bool pickup);
    public event OnPickup onPickup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.gameObject.FindComponent<PlayerMovement>();
        if (playerMovement)
        {
            Pickup(playerMovement, true);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!carrier)
        {
            PlayerMovement playerMovement = collision.gameObject.FindComponent<PlayerMovement>();
            if (playerMovement)
            {
                Pickup(playerMovement, true);
            }
        }
    }
}
