using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MovementLayer
{
    public Vector2 movementVector;

    public MovementLayer(Vector2 movementVector)
    {
        this.movementVector = movementVector;
    }

    public MovementLayer(MovementLayer movementLayer)
    {
        this.movementVector = movementLayer.movementVector;
    }

    public MovementLayer Add(MovementLayer layer)
    {
        MovementLayer ml = new MovementLayer(this);
        ml.movementVector += layer.movementVector;
        return ml;
    }

    public MovementLayer Reverse()
    {
        MovementLayer ml = new MovementLayer(this);
        ml.movementVector *= -1;
        return ml;
    }
}
