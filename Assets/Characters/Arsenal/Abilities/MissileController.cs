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
    [Range(0, 180)]
    [Tooltip("A target must be within this angle in front of the missile in order to be locked onto")]
    public float maxLockOnAngle = 90;
    [Tooltip("The types of entity this missile can lock onto")]
    public List<EntityType> targetableEntityTypes;

    private Vector2? targetPos = null;
    private Rigidbody2D targetObj = null;

    public Vector2? Target
    {
        get => targetPos ?? targetObj?.ClosestPoint(transform.position);
        private set
        {
            targetPos = value;
            if (targetObj)
            {
                targetObj.gameObject.FindComponent<HealthPool>().onDied -= DeselectTarget;
            }
            targetObj = null;
        }
    }

    private void Update()
    {
        if (!PV.IsMine) { return; }
        //Find a target
        if (Target == null)
        {
            List<Rigidbody2D> closeTransforms = new List<Rigidbody2D>();
            RaycastHit2D[] rch2ds = new RaycastHit2D[50];
            int count = Physics2D.CircleCastNonAlloc(transform.position, maxTargetRange, Vector2.zero, rch2ds);
            for (int i = 0; i < count; i++)
            {
                RaycastHit2D rch2d = rch2ds[i];
                //Don't lock onto teammate
                if (TeamToken.onSameTeam(gameObject, rch2d.collider.gameObject))
                {
                    continue;
                }
                //Test this entity to see if it can be locked onto
                HealthPool hp = rch2d.collider.gameObject.FindComponent<HealthPool>();
                if (hp && hp.Health > 0)
                {
                    if (targetableEntityTypes.Contains(hp.entityType))
                    {
                        closeTransforms.Add(hp.gameObject.FindComponent<Rigidbody2D>());
                    }
                }
            }
            if (closeTransforms.Count > 0)
            {
                //Target the closest object in range
                targetObj = closeTransforms
                    .FindAll(rb2d =>
                        Vector2.Angle(rb2d.ClosestPoint(transform.position), transform.position)
                        <= maxLockOnAngle
                        )
                    .OrderBy(rb2d =>
                        Vector2.Distance(rb2d.ClosestPoint(transform.position), transform.position)
                    )
                    .First();
                HealthPool hp = targetObj.gameObject.FindComponent<HealthPool>();
                hp.onDied -= DeselectTarget;
                hp.onDied += DeselectTarget;
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

    void DeselectTarget(float hp)
    {
        Target = null;
    }
}
