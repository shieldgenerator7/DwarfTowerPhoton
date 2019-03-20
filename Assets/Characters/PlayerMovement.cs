using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=JjfPaY57dDM

    public float movementSpeed = 4;

    private Vector2 prevVelocity;

    private PhotonView PV;
    private Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponentInParent<PhotonView>();
        rb2d = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            BasicMovement();
        }
    }

    void BasicMovement()
    {
        //Get inputs
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        //Declare desired velocity
        Vector2 desiredVelocity = rb2d.velocity;
        //If there are movement inputs,
        if (horizontal != 0 || vertical != 0)
        {
            //Set desired velocity
            desiredVelocity =
                ((Vector2.up * vertical) + (Vector2.right * horizontal)).normalized
                * movementSpeed;
        }
        else
        {
            //Else set no velocity
            desiredVelocity = Vector2.zero;
        }
        //If velocity hasn't changed since last frame,
        if (prevVelocity == rb2d.velocity)
        {
            //Give exactly dsired velocity
            rb2d.velocity = desiredVelocity;
        }
        else
        {
            //Else add desired to current velocity
            rb2d.velocity += desiredVelocity;
        }
        //Record current velocity for next frame
        prevVelocity = rb2d.velocity;
    }
}
