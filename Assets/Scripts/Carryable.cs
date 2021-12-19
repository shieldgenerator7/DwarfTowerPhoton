using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryable : MonoBehaviour
{
    public float carryBuffer = 0.5f;

    private PlayerMovement carrier;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (carrier)
        {
            transform.position = (Vector2)carrier.transform.position
                + (carrier.LastMoveDirection.normalized * carryBuffer);
        }
    }

    public void Pickup(PlayerMovement carrier, bool carry)
    {
        if (carry)
        {
            this.carrier = carrier;
        }
        else
        {
            this.carrier = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.gameObject.FindComponent<PlayerMovement>();
        if (playerMovement)
        {
            Pickup(playerMovement, true);
        }
    }
}
