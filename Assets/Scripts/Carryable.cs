using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Carryable : MonoBehaviour
{
    public float carryBuffer = 0.5f;

    private Vector2 holdDirection = Vector2.zero;

    private PlayerMovement _carrier;
    private PlayerMovement carrier
    {
        get => _carrier;
        set
        {
            if (_carrier)
            {
                _carrier.gameObject.FindComponent<StatusKeeper>()
                    .onStatusChanged -= CheckDrop;
            }
            _carrier = value;
            if (_carrier)
            {
                _carrier.gameObject.FindComponent<StatusKeeper>()
                    .onStatusChanged += CheckDrop;
            }
        }
    }
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

    private void CheckDrop(StatusLayer status)
    {
        Drop(status.stunned);
    }

    private void Drop(bool drop)
    {
        Pickup(carrier, !drop);
    }

    public void Pickup(PlayerMovement carrier, bool carry)
    {
        if (carry)
        {
            //Make carrier drop any other item
            FindObjectsOfType<Carryable>().ToList()
                .FindAll(item => item.carrier == carrier)
                .ForEach(item => item.Drop(true));
            //Get picked up
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
        CheckPickup(collision.gameObject);
    }
    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (!carrier)
    //    {
    //        CheckPickup(collision.gameObject);
    //    }
    //}
    private void CheckPickup(GameObject go)
    {
        PlayerMovement playerMovement = go.FindComponent<PlayerMovement>();
        if (playerMovement)
        {
            StatusKeeper statusKeeper = go.FindComponent<StatusKeeper>();
            if (statusKeeper && !statusKeeper.Status.stunned)
            {
                Pickup(playerMovement, true);
            }
        }
    }
}
