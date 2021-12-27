using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriedShotController : ShotController
{
    [SerializeField]
    [Tooltip("The max stats this shot can reach at full charge")]
    private StatLayer statMax = new StatLayer();
    [Tooltip("Max time until the carried shot reaches max level")]
    public float maxTime = 5;
    [Tooltip("The initial position data")]
    public CarriedShotControllerData dataBase;
    [Tooltip("The final position data")]
    public CarriedShotControllerData dataFinal;
    private CarriedShotControllerData dataCurrent;

    private float carryPercent = -1;//how much has been swung
    public float CarryPercent
    {
        get { return carryPercent; }
        set
        {
            float newValue = Mathf.Clamp(value, 0, 1);
            if (carryPercent != newValue)
            {
                carryPercent = newValue;
                dataCurrent.positionAngle = Mathf.Lerp(dataBase.positionAngle, dataFinal.positionAngle, carryPercent);
                dataCurrent.rotationAngle = Mathf.Lerp(dataBase.rotationAngle, dataFinal.rotationAngle, carryPercent);
                dataCurrent.holdBuffer = Mathf.Lerp(dataBase.holdBuffer, dataFinal.holdBuffer, carryPercent);
                dataCurrent.size = Mathf.Lerp(dataBase.size, dataFinal.size, carryPercent);
                stats = StatLayer.Lerp(statBase, statMax, carryPercent);
            }
        }
    }

    public float CarryTime
    {
        get
        {
            if (carryStartTime > 0)
            {
                return Time.time - carryStartTime;
            }
            return -1;
        }
    }
    private float carryStartTime = -1;

    public Vector2 PivotPoint => owner.SpawnCenter;

    private Vector3 origScale;

    private SpriteRenderer sr;

    protected override void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
        dataCurrent = new CarriedShotControllerData();
        CarryPercent = 0;
        origScale = transform.localScale;
        carryStartTime = Time.time;
        if (owner)
        {
            destroyOnIndestructible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (owner)
        {
            if (owner.PV.IsMine)
            {
                //Swing percent
                CarryPercent = CarryTime / maxTime;
                //Position
                Vector2 pointDir = PointDirection;
                pointDir = pointDir.Rotate(dataCurrent.positionAngle).normalized;
                transform.position = PivotPoint + (pointDir * dataCurrent.holdBuffer);
                //Rotation
                Vector2 lookDir = pointDir * ((sr.flipY) ? -1 : 1);
                lookDir = lookDir.Rotate(dataCurrent.rotationAngle).normalized;
                transform.up = lookDir;
                //Scale
                transform.localScale = origScale * dataCurrent.size;
            }
        }
    }

    public void release()
    {
        rb2d.velocity = PointDirection.normalized * owner.playerMovement.rb2d.velocity.magnitude;
        destroyOnIndestructible = true;
        switchOwner(null);
    }

    Vector2 PointDirection
        => (owner.playerMovement.rb2d.isMoving())
        ? owner.playerMovement.LastMoveDirection
        : owner.LookDirection;
}
