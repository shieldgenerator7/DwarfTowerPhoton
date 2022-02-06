using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusKeeper : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Set these to true to allow them to be used as a status effect")]
    private StatusLayer allowedStatus = new StatusLayer();
    public StatusLayer AllowedStatus
    {
        get => allowedStatus;
        set
        {
            StatusLayer prevAllowed = allowedStatus;
            allowedStatus = value;
            if (prevAllowed != allowedStatus)
            {
                updateStatus();
            }
        }
    }

    private Dictionary<int, StatusLayer> stacks = new Dictionary<int, StatusLayer>();

    public StatusLayer Status
    {
        get => currentStatus;
        private set
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
        status.checkValid();
        stacks[id] = status;
        updateStatus();
    }

    public void removeLayer(int id)
    {
        stacks.Remove(id);
        updateStatus();
    }

    private void updateStatus()
    {
        StatusLayer stackLayer = new StatusLayer();
        foreach (StatusLayer layer in stacks.Values)
        {
            stackLayer = stackLayer.stackOr(layer);
        }
        stackLayer = stackLayer.stackAnd(allowedStatus);
        Status = stackLayer;
    }

    /// <summary>
    /// Used to unset a status effect if the message to unset it didn't get received
    /// </summary>
    /// <param name="effect"></param>
    public void BackupUnset(StatusEffect effect)
    {
        foreach (StatusLayer layer in stacks.Values)
        {
            layer.Set(effect, false);
        }
        updateStatus();
    }
}
