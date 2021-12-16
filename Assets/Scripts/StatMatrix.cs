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
    /// <summary>
    /// The layers to add to the base to get the current
    /// </summary>
    private Dictionary<int, StatLayer> addends = new Dictionary<int, StatLayer>();

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

    public void addLayerAdd(int id, StatLayer addend)
    {
        addends[id] = addend;
        updateStats();
    }

    public void removeLayerAdd(int id)
    {
        addends.Remove(id);
        updateStats();
    }

    private void updateStats()
    {
        StatLayer composite = statBase;
        //Multipliers
        foreach (StatLayer layer in multipliers.Values)
        {
            composite = composite.Multiply(layer);
        }
        //Addends
        foreach (StatLayer layer in addends.Values)
        {
            composite = composite.Add(layer);
        }
        //Update
        Stats = composite;
    }

    public void triggerEvent()
    {
        updateStats();
    }
}
