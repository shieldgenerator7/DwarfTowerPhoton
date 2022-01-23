using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private List<Timer> timers = new List<Timer>();

    private static TimerManager instance;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timers.Count > 0)
        {
            timers.ForEach(timer => timer.Update(Time.time));
            timers.RemoveAll(timer => timer.Completed);
        }
    }

    public static Timer StartTimer(float duration)
    {
        Timer timer = new Timer(duration, Time.time);
        instance.timers.Add(timer);
        return timer;
    }

    public static Timer StartTimer(float duration, Timer.OnTimerCompleted completedDelegate)
    {
        Timer timer = StartTimer(duration);
        timer.onTimerCompleted += completedDelegate;
        return timer;
    }

}
