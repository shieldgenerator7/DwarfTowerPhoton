using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatMatrix
{
    [SerializeField]
    [Tooltip("The base stats. Does not change during runtime")]
    private StatLayer statBase = new StatLayer();

    /// <summary>
    /// The current stats
    /// </summary>
    public StatLayer Stats
    {
        get => statCurrent;
        set
        {
            statCurrent = value;
            onStatChanged?.Invoke(Stats);
        }
    }
    private StatLayer statCurrent;
    public delegate void OnStatChanged(StatLayer layer);
    public event OnStatChanged onStatChanged;

    /// <summary>
    /// The layers to multiply to the base to get the current
    /// </summary>
    private Dictionary<int, StatLayer> multipliers = new Dictionary<int, StatLayer>();

    public void init()
    {
        updateStats();
    }

    public void addLayer(int id, StatLayer multiplier)
    {
        multipliers[id] = multiplier;
        updateStats();
    }

    public void removeLayer(int id)
    {
        multipliers.Remove(id);
        updateStats();
    }

    private void updateStats()
    {
        StatLayer composite = statBase;
        foreach (StatLayer layer in multipliers.Values)
        {
            composite = composite.Multiply(layer);
        }
        Stats = composite;
    }

    public void triggerEvent()
    {
        updateStats();
    }
}
