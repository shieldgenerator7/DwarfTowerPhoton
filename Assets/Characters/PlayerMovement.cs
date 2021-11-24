using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=JjfPaY57dDM

    private float movementSpeed = 4;

    /// <summary>
    /// The direction the player is forced to choose
    /// </summary>
    public Vector2 ForceMoveDirection { get; private set; }
    private bool forceMovementInput = false;

    private Vector2 prevVelocity;
    public Vector2 LastMoveDirection { get; private set; }

    private PhotonView PV;
    private Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponentInParent<PhotonView>();
        rb2d = GetComponentInParent<Rigidbody2D>();
        GetComponent<StatKeeper>().selfStats.onStatChanged += (stats) =>
        {
            movementSpeed = stats.moveSpeed;
        };
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
        //Declare desired velocity
        Vector2 desiredVelocity;
        if (forceMovementInput)
        {
            //Move in the force movement direction
            desiredVelocity = ForceMoveDirection * movementSpeed;
        }
        else
        {
            //Get inputs
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            //If there are movement inputs,
            if (horizontal != 0 || vertical != 0)
            {
                //Set desired velocity
                desiredVelocity =
                    ((Vector2.up * vertical) + (Vector2.right * horizontal)).normalized
                    * movementSpeed;
            }
            else if (Input.GetButton("MoveToCursor"))
            {
                desiredVelocity =
                    ((Vector2)(Utility.MouseWorldPos - transform.position)).normalized
                    * movementSpeed;
            }
            else
            {
                //Else set no velocity
                desiredVelocity = Vector2.zero;
            }
        }
        //If velocity hasn't changed since last frame,
        if (prevVelocity == rb2d.velocity)
        {
            //Give exactly desired velocity
            rb2d.velocity = desiredVelocity;
        }
        else
        {
            //Else add desired to current velocity
            rb2d.velocity += desiredVelocity;
        }
        //Record current velocity for next frame
        prevVelocity = rb2d.velocity;
        if (rb2d.velocity != Vector2.zero)
        {
            LastMoveDirection = rb2d.velocity;
        }
    }

    public void forceMovement(bool force)
    {
        forceMovement(LastMoveDirection, force);
    }

    public void forceMovement(Vector2 direction, bool force = true)
    {
        ForceMoveDirection = direction.normalized;
        forceMovementInput = force;
        LastMoveDirection = direction;
    }
}
