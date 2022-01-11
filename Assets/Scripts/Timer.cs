using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float duration;
    private float startTime;
    private float time;

    public bool Completed => time >= startTime + duration;

    public Timer(float duration, float time)
    {
        this.duration = duration;
        this.startTime = time;
        this.time = time;
    }

    public void Update(float time)
    {
        this.time = time;
        if (Completed)
        {
            onTimerCompleted?.Invoke();
        }
    }
    public delegate void OnTimerCompleted();
    public event OnTimerCompleted onTimerCompleted;
}
