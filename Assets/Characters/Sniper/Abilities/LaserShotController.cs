using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotController : ShotController
{
    [Tooltip("The initial width it is when it closes in")]
    public float startWidth = 1;
    [Tooltip("The final width it is when it deals damage")]
    public float endWidth = 0.1f;
    [Tooltip("The amount of seconds to deal damage after releasing")]
    public float closeInDuration = 0.5f;
    [Tooltip("The amount of seconds it stays after starting to deal damage")]
    public float stayDuration = 0.2f;
    [Tooltip("How far the laser goes")]
    public float range = 100;

    private bool readyToFire = false;
    public bool ReadyToFire
    {
        get => readyToFire;
        set
        {
            readyToFire = value;
            fireStartTime = Time.time;
        }
    }

    private Vector2 _startPos;
    public Vector2 StartPos
    {
        get => _startPos;
        set
        {
            _startPos = value;
            recalculatePositions();
        }
    }
    private Vector2 _endPos;
    public Vector2 EndPos
    {
        get => _endPos;
        private set
        {
            _endPos = value;
            //recalculatePositions();
        }
    }
    private Vector2 _dir;
    public Vector2 Direction
    {
        get => _dir;
        set
        {
            _dir = value.normalized;
            recalculatePositions();
        }
    }

    private void recalculatePositions()
    {
        //Calculate EndPos
        EndPos = StartPos + (Direction * range);
        //Reposition
        transform.position = StartPos;
        Vector3 scale = transform.localScale;
        scale.y = (EndPos - StartPos).magnitude;
        transform.localScale = scale;
        transform.up = Direction;
    }

    private void updateWidth()
    {
        float percent = Mathf.Clamp(
            ((fireStartTime + closeInDuration) - Time.time) / closeInDuration,
            0,
            1
            );
        Vector3 scale = transform.localScale;
        scale.x = ((endWidth - startWidth) * percent) + startWidth;
        transform.localScale = scale;
    }

    private float fireStartTime = -1;

    private Collider2D coll2d;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        coll2d = gameObject.FindComponent<Collider2D>();
        coll2d.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireStartTime >= 0)
        {
            updateWidth();
            if (Time.time >= fireStartTime + closeInDuration)
            {
                if (!coll2d.enabled)
                {
                    coll2d.enabled = true;
                }
                if (Time.time >= fireStartTime + closeInDuration + stayDuration)
                {
                    onLaserEnded?.Invoke();
                    if (PV.IsMine)
                    {
                        PhotonNetwork.Destroy(PV.gameObject);
                    }
                }
            }
        }
    }
    public delegate void LaserEvent();
    public event LaserEvent onLaserEnded;
}
