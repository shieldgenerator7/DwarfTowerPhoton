using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPunObservable
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=JjfPaY57dDM

    [Tooltip("Should this movement conserve some previous momentum when changing directions?")]
    public bool floaty = false;
    [Tooltip("If floaty, how fast it should lerp movement")]
    public float lerpSpeed = 1;

    public float MovementSpeed { get; set; } = 4;

    /// <summary>
    /// The direction the player is forced to choose
    /// </summary>
    public Vector2 ForceMoveDirection { get; private set; }
    private bool forceMovementInput = false;
    public bool ForcingMovement => forceMovementInput;

    private Vector2 moveVelocity = Vector2.zero;
    private Vector2 prevVelocity;
    public Vector2 LastMoveDirection { get; private set; }

    private ComponentContext compContext;

    // Start is called before the first frame update
    public void Start()
    {
        compContext = gameObject.FindComponent<ComponentContext>();
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
            //Lerp velocity
            moveVelocity = Vector2.Lerp(moveVelocity, desiredVelocity, lerpSpeed * Time.deltaTime);
            if (Vector2.Distance(moveVelocity, desiredVelocity) < 0.01f)
            {
                moveVelocity = desiredVelocity;
            }
        }
        else
        {
            //Give exactly desired velocity
            moveVelocity = desiredVelocity;
        }
        //Record current velocity for next frame
        if (prevVelocity != moveVelocity)
        {
            compContext.movementKeeper.AddLayer(
                compContext.PV.ViewID,
                new MovementLayer(moveVelocity)
                );
        }
        prevVelocity = moveVelocity;
        if (moveVelocity != Vector2.zero)
        {
            LastMoveDirection = moveVelocity;
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
