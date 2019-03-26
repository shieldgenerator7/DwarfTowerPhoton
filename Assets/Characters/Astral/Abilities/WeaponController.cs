using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : ChargedShotController
{
    public GameObject wielder;
    public WeaponControllerData dataBase;
    public WeaponControllerData dataFinal;
    private WeaponControllerData dataCurrent;
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

    protected override void Start()
    {
        base.Start();
        sr = GetComponent<SpriteRenderer>();
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
                if (Input.GetButton("Ability1"))
                {
                    SwingPercent += Time.deltaTime;
                }
                else
                {
                    SwingPercent -= Time.deltaTime;
                }
                Vector2 mouseDir = (Vector2)Utility.MouseWorldPos - PivotPoint;
                Vector2 pointDir = mouseDir.Rotate(dataCurrent.positionAngle).normalized;
                transform.position = PivotPoint + (pointDir * dataCurrent.holdBuffer);
                Vector2 lookDir = pointDir * ((sr.flipY) ? -1 : 1);
                lookDir = lookDir.Rotate(dataCurrent.rotationAngle).normalized;
                transform.up = lookDir;
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
            }
        }
        else
        {
            processCollision(collision, true);
        }
    }
}
