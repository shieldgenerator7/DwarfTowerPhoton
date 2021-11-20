using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriedShotController : ChargedShotController
{
    public PlayerController wielder;
    public CarriedShotControllerData dataBase;
    public CarriedShotControllerData dataFinal;
    private CarriedShotControllerData dataCurrent;
    public float swingSpeed = 1;
    public float throwSpeed = 3;

    private float swingPercent = -1;//how much has been swung
    public float SwingPercent
    {
        get { return swingPercent; }
        set
        {
            float newValue = Mathf.Clamp(value, 0, 1);
            if (swingPercent != newValue)
            {
                swingPercent = newValue;
                dataCurrent.positionAngle = Mathf.Lerp(dataBase.positionAngle, dataFinal.positionAngle, swingPercent);
                dataCurrent.rotationAngle = Mathf.Lerp(dataBase.rotationAngle, dataFinal.rotationAngle, swingPercent);
                dataCurrent.holdBuffer = Mathf.Lerp(dataBase.holdBuffer, dataFinal.holdBuffer, swingPercent);
            }
        }
    }

    public Vector2 PivotPoint
    {
        get
        {
            if (wielder)
            {
                Vector2 wielderCenter = (Vector2)wielder.transform.position + (Vector2.up * 0.5f);
                return wielderCenter;
            }
            else
            {
                return transform.position;
            }
        }
    }

    private SpriteRenderer sr;
    private PlayerMovement playerMovement;

    protected override void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
        dataCurrent = new CarriedShotControllerData();
        SwingPercent = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (wielder)
        {
            if (wielder.PV.IsMine)
            {
                //
                // Swing
                //
                if (Input.GetButton("Ability1"))
                {
                    SwingPercent += swingSpeed * Time.deltaTime;
                }
                else
                {
                    SwingPercent -= swingSpeed * Time.deltaTime;
                }
                Vector2 pointDir = playerMovement.LastMoveDirection.Rotate(dataCurrent.positionAngle).normalized;
                transform.position = PivotPoint + (pointDir * dataCurrent.holdBuffer);
                Vector2 lookDir = pointDir * ((sr.flipY) ? -1 : 1);
                lookDir = lookDir.Rotate(dataCurrent.rotationAngle).normalized;
                transform.up = lookDir;
                //
                // Throw
                //
                if (Input.GetButtonDown("Ability2"))
                {
                    switchOwner(null);
                    rb2d.velocity = pointDir * throwSpeed;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool onSameTeam = TeamToken.onSameTeam(gameObject, collision.gameObject);
        if (onSameTeam)
        {
            if (wielder == null)
            {
                bool targetIsPlayer = collision.gameObject.CompareTag("Player");
                if (targetIsPlayer)
                {
                    wielder = collision.gameObject.GetComponent<PlayerController>();
                    rb2d.velocity = Vector2.zero;
                    //Photon Take Over
                    if (PV.IsMine)
                    {
                        switchOwner(wielder);
                    }
                }
            }
        }
        else
        {
            rb2d.velocity = Vector2.zero;
            processCollision(collision, true);
        }
    }

    void switchOwner(PlayerController pc)
    {
        int ownerID = -1;
        if (pc)
        {
            PV.TransferOwnership(pc.PV.Owner);
            ownerID = pc.PV.ViewID;
        }
        PV.RPC("RPC_SwitchOwner", RpcTarget.AllBuffered, ownerID);
    }

    [PunRPC]
    void RPC_SwitchOwner(int ownerID)
    {
        wielder = null;
        if (ownerID >= 0)
        {
            foreach (PlayerController pc in FindObjectsOfType<PlayerController>())
            {
                if (pc.PV.ViewID == ownerID)
                {
                    wielder = pc;
                    playerMovement = wielder.GetComponent<PlayerMovement>();
                }
            }
        }
    }
}
