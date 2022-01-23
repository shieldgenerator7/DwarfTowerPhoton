using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAngleChanger : MonoBehaviour
{
    [Tooltip("How much to angle each additional bullet by")]
    public float angleDelta = 90;
    [Tooltip("How many seconds it should wait after the last bullet fires before resetting the angle")]
    public float resetWaitDuration = 2;

    public GunController gunController;

    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        gunController.onShotFired += rotateGun;
    }

    void rotateGun(GameObject shot, Vector2 targetPos, Vector2 targetDir)
    {
        gunController.angle += angleDelta;
        startTimer();
    }

    void startTimer()
    {
        if (timer == null)
        {
            timer = TimerManager.StartTimer(resetWaitDuration);
            timer.onTimerCompleted += () =>
            {
                gunController.angle = 0;
                timer = null;
            };
        }
        else
        {
            timer.reset();
        }
    }
}
