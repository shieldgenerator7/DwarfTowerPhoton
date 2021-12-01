using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotController : ShotController
{
    [Range(0, 2)]
    [Tooltip("The initial sprite width it is when it closes in")]
    public float startWidth = 1;
    [Range(0, 2)]
    [Tooltip("The final sprite width it is when it deals damage")]
    public float endWidth = 0.1f;
    [Range(0, 1)]
    [Tooltip("The alpha while closing in")]
    public float startAlpha = 0.1f;
    [Range(0, 1)]
    [Tooltip("The alpha while dealing damage")]
    public float endAlpha = 1;
    [Range(0, 2)]
    [Tooltip("The amount of seconds to deal damage after releasing")]
    public float closeInDuration = 0.5f;
    [Range(0, 2)]
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
            1 - (((fireStartTime + closeInDuration) - Time.time) / closeInDuration),
            0,
            1
            );
        //Sprite Width
        float widthDiff = ((endWidth - startWidth) * percent);
        float width = widthDiff + startWidth;
        Vector2 size = sr.size;
        size.x = width;
        sr.size = size;
        //Sprite Alpha
        float alpha = (percent == 1) ? endAlpha : startAlpha;
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }

    private float fireStartTime = -1;

    private Collider2D coll2d;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        coll2d = gameObject.FindComponent<Collider2D>();
        coll2d.enabled = false;
        sr = gameObject.FindComponent<SpriteRenderer>();
        //Start Width
        Vector2 size = sr.size;
        size.x = startWidth;
        sr.size = size;
        //Start alpha
        Color color = sr.color;
        color.a = startAlpha;
        sr.color = color;
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
                    dealInitialDamage();
                }
                if (Time.time >= fireStartTime + closeInDuration + stayDuration)
                {
                    destroy();
                }
            }
        }
    }
    public delegate void LaserEvent();
    public event LaserEvent onLaserEnded;

    public void destroy()
    {
        onLaserEnded?.Invoke();
        if (PV.IsMine)
        {
            PhotonNetwork.Destroy(PV.gameObject);
        }
    }

    void dealInitialDamage()
    {
        RaycastHit2D[] rch2d = new RaycastHit2D[100];
        int count = coll2d.Cast(transform.up, rch2d, range, true);
        for (int i = 0; i < count; i++)
        {
            damager.processCollision(rch2d[i].collider, true);
        }
    }
}
