using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPunObservable
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=JjfPaY57dDM

    [Tooltip("Should this movement conserve some previous momentum when changing directions?")]
    public bool floaty = false;

    public float MovementSpeed { get; set; } = 4;

    /// <summary>
    /// The direction the player is forced to choose
    /// </summary>
    public Vector2 ForceMoveDirection { get; private set; }
    private bool forceMovementInput = false;
    public bool ForcingMovement => forceMovementInput;

    private Vector2 prevVelocity;
    public Vector2 LastMoveDirection { get; private set; }

    public Rigidbody2D rb2d { get; private set; }

    // Start is called before the first frame update
    public void Start()
    {
        rb2d = gameObject.FindComponent<Rigidbody2D>();
    }

    public void BasicMovement(InputState inputState)
    {
        //Declare desired velocity
        Vector2 desiredVelocity;
        if (forceMovementInput)
        {
            //Move in the force movement direction
            desiredVelocity = ForceMoveDirection * MovementSpeed;
        }
        else
        {
            //If there are movement inputs,
            if (inputState.movement.magnitude > 0)
            {
                //Set desired velocity
                desiredVelocity = inputState.movement.normalized * MovementSpeed;
            }
            else if (inputState.moveTowardsCursor.Bool())
            {
                desiredVelocity =
                    ((Vector2)(Utility.MouseWorldPos - transform.position)).normalized
                    * MovementSpeed;
            }
            else
            {
                //Else set no velocity
                desiredVelocity = Vector2.zero;
            }
        }
        if (floaty)
        {
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
        }
        else
        {
            //Give exactly desired velocity
            rb2d.velocity = desiredVelocity;
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
        forceMovement(
            (force) ? LastMoveDirection : Vector2.zero,
            force
            );
    }

    public void forceMovement(Vector2 direction, bool force = true)
    {
        ForceMoveDirection = direction.normalized;
        forceMovementInput = force;
        if (direction.magnitude > 0)
        {
            LastMoveDirection = direction;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(LastMoveDirection);
        }
        else
        {
            LastMoveDirection = (Vector2)stream.ReceiveNext();
        }
    }
}
