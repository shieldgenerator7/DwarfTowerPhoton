using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanController : PlayerController
{
    [Tooltip("Size when it's at 1 hp")]
    public float minSize = 0.5f;
    [Tooltip("Speed when it has lots of hp")]
    public float minSpeed = 1;
    [Tooltip("Speed when it's at 1 hp")]
    public float maxSpeed = 5;

    private StatLayer speedLayer;

    protected override void Start()
    {
        base.Start();
        if (PV.IsMine)
        {
            healthPool.onChanged += (hp) =>
            {
                UpdateSize(hp);
                UpdateSpeed(hp);
            };
            speedLayer = new StatLayer();
            speedLayer.moveSpeed = minSpeed;
            //Init values
            UpdateSize(healthPool.Health);
            UpdateSpeed(healthPool.Health);
        }
    }

    private void UpdateSize(float health)
    {
        //Increase size with health
        float scale = Mathf.Max(minSize, health);
        transform.localScale = Vector3.one * scale;
    }
    private void UpdateSpeed(float health)
    {
        //Decrease speed with health
        float speed = Mathf.Max(minSpeed, maxSpeed - (health - 1));
        speedLayer.moveSpeed = speed;
        statKeeper.selfStats.addLayerAdd(PV.ViewID, speedLayer);
    }
}
