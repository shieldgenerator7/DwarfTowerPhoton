using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Sometimes there's networking issues that result in players getting indefinitely
/// stunned, rooted, or other status effect.
/// This script will make the effect end automatically in case the status effect
/// message gets lost in the network.
/// </summary>
public class StatusAutoEnder : MonoBehaviour
{
    public float defaultMaxDuration = 10;
    
    [System.Serializable]
    public struct StatusTimeData
    {
        public StatusEffect statusEffect;
        public float maxDuration;
    }
    public List<StatusTimeData> statusDurations;

    private Dictionary<StatusEffect, Timer> statusTimers = new Dictionary<StatusEffect, Timer>();

    private StatusKeeper statusKeeper;

    public void CheckStatusTimers(StatusLayer status)
    {
        CheckStatusTimer(StatusEffect.STUNNED, status.stunned);
        CheckStatusTimer(StatusEffect.ROOTED, status.rooted);
        CheckStatusTimer(StatusEffect.STEALTHED, status.stealthed);
    }

    private void CheckStatusTimer(StatusEffect status, bool statusOn)
    {
        if (statusOn)
        {
            if (!statusTimers.ContainsKey(status))
            {
                float maxDuration = defaultMaxDuration;
                if (statusDurations.Any(sd => sd.statusEffect == status))
                {
                    maxDuration = statusDurations.First(
                        sd => sd.statusEffect == status
                    ).maxDuration;
                }
                Timer timer = TimerManager.StartTimer(maxDuration);
                timer.onTimerCompleted += () =>
                {
                    statusTimers.Remove(status);
                };
                statusTimers.Add(status, timer);
            }
        }
    }
}
