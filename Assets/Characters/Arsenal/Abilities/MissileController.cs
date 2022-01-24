using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissileController : ShotController
{
    [Tooltip("How many degrees this missile can turn each second")]
    public float maxTurnAnglePerSecond = 2;
    [Tooltip("If the target is outside this range, the missile stops tracking toward it")]
    public float maxTargetRange = 7;
    [Tooltip("The types of entity this missile can lock onto")]
    public List<EntityType> targetableEntityTypes;
    [Tooltip("The collider used to detect entities it can lock onto")]
    public Collider2D targetFindColl;

    private Vector2? targetPos = null;
    private Transform targetObj = null;

    public Vector2? Target
    {
        get => targetPos ?? targetObj?.position;
        private set => targetPos = value;
    }

    private void Update()
    {
        if (!PV.IsMine) { return; }
        //Find a target
        if (Target == null)
        {
            List<Transform> closeTransforms = new List<Transform>();
            RaycastHit2D[] rch2ds = new RaycastHit2D[50];
            int count = targetFindColl.Cast(Vector2.zero, rch2ds);
            for (int i = 0; i < count; i++)
            {
                RaycastHit2D rch2d = rch2ds[i];
                HealthPool hp = rch2d.collider.gameObject.FindComponent<HealthPool>();
                if (hp)
                {
                    if (targetableEntityTypes.Contains(hp.entityType))
                    {
                        closeTransforms.Add(hp.gameObject.FindComponent<Rigidbody2D>().transform);
                    }
                }
            }
            if (closeTransforms.Count > 0)
            {
                //Target the closest object in range
                targetObj = closeTransforms
                    .OrderBy(t =>
                        Vector2.Distance(t.position, transform.position)
                    )
                    .First();
            }
        }
        //Move towards the target
        if (Target != null)
        {
            Vector2 targetDir = (Vector2)Target - (Vector2)transform.position;
            //If target is still in range
            if (targetDir.magnitude <= maxTargetRange)
            {
                //Turn toward it
                Vector2 moveDir = transform.up.normalized;
                targetDir.Normalize();
                if (moveDir != targetDir)
                {
                    float angleDir = Mathf.Sign(Vector2.SignedAngle(moveDir, targetDir));
                    moveDir = Quaternion.Euler(
                        0,
                        0,
                        maxTurnAnglePerSecond * angleDir * Time.deltaTime
                        ) * moveDir;
                    //Change direction
                    transform.up = moveDir;
                    rb2d.velocity = moveDir * rb2d.velocity.magnitude;
                }
            }
            else
            {
                //Target out of range, stop tracking it
                Target = null;
            }
        }


    }
}
