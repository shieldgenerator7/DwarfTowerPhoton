using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : ChargedShotController
{
    public GameObject wielder;
    public float positionAngle = 0;//the angle between the facing direction and the weapon's hold direction
    public float rotationAngle = 0;//the angle between the hold direction and the weapon's rotation
    public float holdBuffer = 1;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            if (wielder)
            {
                Vector2 mouseDir = (Vector2)Utility.MouseWorldPos - PivotPoint;
                Vector2 pointDir = mouseDir.Rotate(positionAngle).normalized;
                transform.position = PivotPoint + (pointDir * holdBuffer);
                Vector2 lookDir = (pointDir * ((sr.flipY) ? -1 : 1)).Rotate(rotationAngle).normalized;
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
