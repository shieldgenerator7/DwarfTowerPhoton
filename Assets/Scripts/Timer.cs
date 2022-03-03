using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float duration;
    private float startTime;
    private float time;

    private bool shouldReset = false;
    private bool shouldCancel = false;

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
        if (shouldReset)
        {
            shouldReset = false;
            this.startTime = time;
        }
        if (Completed)
        {
            onTimerCompleted?.Invoke();
        }
        if (shouldCancel)
        {
            this.time = time;
            this.startTime = time;
            this.duration = 0;
        }
    }
    public delegate void OnTimerCompleted();
    public event OnTimerCompleted onTimerCompleted;

    public void reset()
    {
        shouldReset = true;
    }

    public void cancel()
    {
        shouldCancel = true;
    }
}
