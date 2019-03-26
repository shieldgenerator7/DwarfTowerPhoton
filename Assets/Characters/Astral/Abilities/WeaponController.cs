﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : ChargedShotController
{
    public GameObject wielder;
    public WeaponControllerData dataBase;
    public WeaponControllerData dataFinal;
    private WeaponControllerData dataCurrent;
    public float swingSpeed = 1;
    public float throwSpeed = 3;

    private float swingPercent = -1;//how much has been swung
    public float SwingPercent
    {
        get { return swingPercent; }
        set
        {
            float newValue = Mathf.Clamp(value, 0, 1);
            if (swingPercent != newValue) {
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
    private Rigidbody2D rb2d;

    protected override void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        dataCurrent = new WeaponControllerData();
        SwingPercent = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            if (wielder)
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
                Vector2 mouseDir = (Vector2)Utility.MouseWorldPos - PivotPoint;
                Vector2 pointDir = mouseDir.Rotate(dataCurrent.positionAngle).normalized;
                transform.position = PivotPoint + (pointDir * dataCurrent.holdBuffer);
                Vector2 lookDir = pointDir * ((sr.flipY) ? -1 : 1);
                lookDir = lookDir.Rotate(dataCurrent.rotationAngle).normalized;
                transform.up = lookDir;
                //
                // Throw
                //
                if (Input.GetButtonDown("Ability2"))
                {
                    wielder = null;
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
            bool targetIsPlayer = collision.gameObject.CompareTag("Player");
            if (targetIsPlayer)
            {
                wielder = collision.gameObject;
                rb2d.velocity = Vector2.zero;
            }
        }
        else
        {
            rb2d.velocity = Vector2.zero;
            processCollision(collision, true);
        }
    }
}
