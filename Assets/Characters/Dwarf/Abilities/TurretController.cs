using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : GunController
{

    private ShotController sc;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        sc = GetComponentInParent<ShotController>();
    }

    public void fireInDirection(GameObject shot, Vector2 targetPos, Vector2 targetDir)
    {
        fireShot(transform.position, (Vector2)transform.position + targetDir);
    }
}
