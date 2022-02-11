using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : ChargedShotController
{
    public ShotControllerData dataBase;
    public ShotControllerData dataFinal;
    private HoldShotData dataCurrent;
    public float swingSpeed = 1;
    public float throwSpeed = 3;

    public AbilitySlot swingAbilitySlot = AbilitySlot.Ability1;
    public AbilitySlot throwAbilitySlot = AbilitySlot.Ability2;

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
                dataCurrent.holdAngle = Mathf.Lerp(dataBase.holdShotData.holdAngle, dataFinal.holdShotData.holdAngle, swingPercent);
                dataCurrent.rotationAngle = Mathf.Lerp(dataBase.holdShotData.rotationAngle, dataFinal.holdShotData.rotationAngle, swingPercent);
                dataCurrent.holdBuffer = Mathf.Lerp(dataBase.holdShotData.holdBuffer, dataFinal.holdShotData.holdBuffer, swingPercent);
            }
        }
    }

    public Vector2 PivotPoint
    {
        get
        {
            if (teamToken.controller != teamToken)
            {
                Vector2 wielderCenter = (Vector2)teamToken.controller.transform.position + (Vector2.up * 0.5f);
                return wielderCenter;
            }
            else
            {
                return transform.position;
            }
        }
    }

    private SpriteRenderer sr;

    protected override void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
        dataCurrent = new HoldShotData();
        SwingPercent = 0;
        teamToken.onControllerLostControl +=
            (controller) => registerDelegates(controller, false);
        teamToken.onControllerGainedControl +=
            (controller) => registerDelegates(controller, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller)
        {
            if (Controller.PV.IsMine)
            {
                //
                // Swing
                //
                if (Controller.inputState.Button(swingAbilitySlot).Bool())
                {
                    SwingPercent += swingSpeed * Time.deltaTime;
                }
                else
                {
                    SwingPercent -= swingSpeed * Time.deltaTime;
                }
                Vector2 mouseDir = (Vector2)Utility.MouseWorldPos - PivotPoint;
                Vector2 pointDir = mouseDir.Rotate(dataCurrent.holdAngle).normalized;
                transform.position = PivotPoint + (pointDir * dataCurrent.holdBuffer);
                Vector2 lookDir = pointDir * ((sr.flipY) ? -1 : 1);
                lookDir = lookDir.Rotate(dataCurrent.rotationAngle).normalized;
                transform.up = lookDir;
                //
                // Throw
                //
                if (Controller.inputState.Button(throwAbilitySlot) == ButtonState.DOWN)
                {
                    teamToken.switchController(null);
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
            if (Controller == null)
            {
                bool targetIsPlayer = collision.gameObject.CompareTag("Player");
                if (targetIsPlayer)
                {
                    PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
                    if (!pc.Stunned)
                    {
                        rb2d.velocity = Vector2.zero;
                        //Photon Take Over
                        if (PV.IsMine)
                        {
                            teamToken.switchController(pc.teamToken);
                        }
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

    void registerDelegates(TeamToken controller, bool register)
    {
        StatusKeeper statusKeeper = controller.gameObject.FindComponent<StatusKeeper>();
        if (statusKeeper)
        {
            statusKeeper.onStatusChanged -= checkReleaseFromOwner;
            if (register)
            {
                statusKeeper.onStatusChanged += checkReleaseFromOwner;
            }
        }
    }

    void checkReleaseFromOwner(StatusLayer status)
    {
        if (status.Has(StatusEffect.STUNNED))
        {
            teamToken.switchController(null);
        }
    }
}
