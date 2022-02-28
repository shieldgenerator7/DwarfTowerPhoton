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
        private set => moveV = value;
    }

    private ComponentContext compContext;

    // Start is called before the first frame update
    void Start()
    {
        compContext = gameObject.FindComponent<ComponentContext>();
    }

    // Update is called once per frame
    void Update()
    {
        compContext.rb2d.velocity = Vector2.zero;
    }

    public void AddLayer(int id, MovementLayer layer)
    {
        layers.Add(id, layer);
    }

    public void RemoveLayer(int id)
    {
        layers.Remove(id);
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
