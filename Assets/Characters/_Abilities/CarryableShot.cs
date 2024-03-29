using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryableShot : MonoBehaviour
{
    public HoldShotData holdShotData;

    private Vector3 origScale;

    public ShotController shotController { get; private set; }
    private SpriteRenderer sr;

    // Start is called before the first frame update
    public void Start()
    {
        shotController = gameObject.FindComponent<ShotController>();
        sr = gameObject.FindComponent<SpriteRenderer>();

        origScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerController Controller = shotController.Controller;
        if (Controller)
        {
            if (Controller.context.PV.IsMine)
            {
                //Position
                Vector2 pointDir = Controller.LookDirection;
                pointDir = pointDir.Rotate(holdShotData.holdAngle).normalized;
                transform.position = Controller.SpawnCenter + (pointDir * holdShotData.holdBuffer);
                //Rotation
                Vector2 lookDir = pointDir * ((sr.flipY) ? -1 : 1);
                lookDir = lookDir.Rotate(holdShotData.rotationAngle).normalized;
                transform.up = lookDir;
                //Scale
                transform.localScale = origScale * holdShotData.size;
            }
        }
    }

    public void release()
    {
        shotController.destroyOnIndestructible = true;
        shotController.Launch();
        shotController.Controller = null;
    }
}
