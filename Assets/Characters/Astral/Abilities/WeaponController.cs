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
            if (owner)
            {
                Vector2 wielderCenter = (Vector2)owner.transform.position + (Vector2.up * 0.5f);
                return wielderCenter;
            }
            else
            {
                return transform.position;
            }
        }
    }

    public override PlayerController owner
    {
        protected set
        {
            if (owner)
            {
                owner.statusKeeper.onStatusChanged -= checkReleaseFromOwner;
            }
            base.owner = value;
            if (owner)
            {
                owner.statusKeeper.onStatusChanged -= checkReleaseFromOwner;
                owner.statusKeeper.onStatusChanged += checkReleaseFromOwner;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (owner)
        {
            if (owner.PV.IsMine)
            {
                //
                // Swing
                //
                if (owner.inputState.Button(swingAbilitySlot).Bool())
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
                if (owner.inputState.Button(throwAbilitySlot) == ButtonState.DOWN)
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
            if (owner == null)
            {
                bool targetIsPlayer = collision.gameObject.CompareTag("Player");
                if (targetIsPlayer)
                {
                    PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
                    if (!pc.Stunned)
                    {
                        owner = pc;
                        rb2d.velocity = Vector2.zero;
                        //Photon Take Over
                        if (PV.IsMine)
                        {
                            switchOwner(owner);
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

    void checkReleaseFromOwner(StatusLayer status)
    {
        if (status.Has(StatusEffect.STUNNED))
        {
            switchOwner(null);
        }
    }
}
