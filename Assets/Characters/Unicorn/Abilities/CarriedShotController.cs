using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriedShotController : ShotController
{
    [SerializeField]
    [Tooltip("The max stats this shot can reach at full charge")]
    private StatLayer statMax = new StatLayer();
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

    public Vector2 PivotPoint => carrier.playerController.SpawnCenter;

    private Vector3 origScale;

    private CarriedGunController carrier;
    private SpriteRenderer sr;

    protected override void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
        dataCurrent = new CarriedShotControllerData();
        CarryPercent = 0;
        origScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (carrier)
        {
            if (carrier.PV.IsMine)
            {
                //Swing percent
                CarryPercent = carrier.CarryTime / carrier.maxTime;
                //Position
                Vector2 pointDir = carrier.playerMovement.LastMoveDirection.Rotate(dataCurrent.positionAngle).normalized;
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
        rb2d.velocity = carrier.playerMovement.LastMoveDirection;
        switchOwner(null);
    }

    public void switchOwner(CarriedGunController cgc)
    {
        int ownerID = -1;
        if (cgc)
        {
            PV.TransferOwnership(cgc.PV.Owner);
            ownerID = cgc.PV.ViewID;
        }
        PV.RPC("RPC_SwitchOwner", RpcTarget.AllBuffered, ownerID);
    }

    [PunRPC]
    void RPC_SwitchOwner(int ownerID)
    {
        carrier = null;
        if (ownerID >= 0)
        {
            foreach (CarriedGunController cgc in FindObjectsOfType<CarriedGunController>())
            {
                if (cgc.PV.ViewID == ownerID)
                {
                    carrier = cgc;
                }
            }
        }
    }
}
