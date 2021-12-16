using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusKeeper : MonoBehaviour
{
    [Tooltip("Set these to true to allow them to be used as a status effect")]
    public StatusLayer allowedStatus;

    private Dictionary<int, StatusLayer> stacks = new Dictionary<int, StatusLayer>();

    public StatusLayer Status
    {
        get => currentStatus;
        set
        {
            currentStatus = value;
            onStatusChanged?.Invoke(currentStatus);
        }
    }
    private StatusLayer currentStatus;
    public delegate void OnStatusChanged(StatusLayer status);
    public event OnStatusChanged onStatusChanged;

    public void addLayer(int id, StatusLayer status)
    {
        stacks[id] = status;
        updateStats();
    }

    public void removeLayer(int id)
    {
        stacks.Remove(id);
        updateStats();
    }

    private void updateStats()
    {
        StatusLayer stackLayer = new StatusLayer();
        foreach (StatusLayer layer in stacks.Values)
        {
            stackLayer = stackLayer.stackOr(layer);
        }
        stackLayer = stackLayer.stackAnd(allowedStatus);
        Status = stackLayer;
    }
}
