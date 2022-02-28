using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementKeeper : MonoBehaviour
{
    private Dictionary<int, MovementLayer> layers = new Dictionary<int, MovementLayer>();

    private Vector2 moveV;
    public Vector2 MovementVector
    {
        get => moveV;
        private set
        {
            moveV = value;
            compContext.rb2d.velocity = moveV;
        }
    }

    private ComponentContext compContext;

    // Start is called before the first frame update
    void Start()
    {
        compContext = gameObject.FindComponent<ComponentContext>();
    }

    public void AddLayer(int id, MovementLayer layer)
    {
        layers.Add(id, layer);
        UpdateMovement();
    }

    public void RemoveLayer(int id)
    {
        layers.Remove(id);
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        MovementLayer layer = new MovementLayer();
        foreach (var entry in layers)
        {
            layer = layer.Add(entry.Value);
        }
        MovementVector = layer.movementVector;
    }
}
