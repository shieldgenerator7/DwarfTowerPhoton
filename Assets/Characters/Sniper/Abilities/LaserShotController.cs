using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotController : ShotController
{
    [Range(0, 2)]
    [Tooltip("The initial width it is when it closes in")]
    public float startWidth = 1;
    [Range(0, 2)]
    [Tooltip("The final width it is when it deals damage")]
    public float endWidth = 0.1f;
    [Range(0, 1)]
    [Tooltip("The initial alpha it is when it closes in")]
    public float startAlpha = 0.1f;
    [Range(0, 1)]
    [Tooltip("The final alpha it is when it deals damage")]
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

    private List<GameObject> dealtInitialDamage = new List<GameObject>();

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
        float widthDiff = ((endWidth - startWidth) * percent);
        float width = widthDiff + startWidth;
        Vector3 scale = transform.localScale;
        scale.x = width;
        transform.localScale = scale;
        //Sprite transparency
        float alphaDiff = ((endAlpha - startAlpha) * percent);
        float alpha = alphaDiff + startAlpha;
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
        Debug.Log("coll2d: " + coll2d);
        sr = gameObject.FindComponent<SpriteRenderer>();
        //Start Width
        Vector3 scale = transform.localScale;
        scale.x = startWidth;
        transform.localScale = scale;
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
                    //gameObject.SetActive(false);
                    coll2d.enabled = true;
                    //gameObject.SetActive(true);
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
        //ContactFilter2D filter = new ContactFilter2D();
        int count = coll2d.Cast(transform.up, rch2d, range, true);
        Debug.Log("found colliders: " + count);
        for (int i = 0; i < count; i++)
        {
            Debug.Log("found collider: " + rch2d[i].collider.name);
            damager.processCollision(rch2d[i].collider, true);
        }
    }

    private void OnTriggerStay2D(Collider2D coll2d)
    {
        if (!dealtInitialDamage.Contains(coll2d.gameObject))
        {
            Debug.Log("found collider: " + coll2d.name);
            dealtInitialDamage.Add(coll2d.gameObject);
            damager.processCollision(coll2d, true);
        }
    }
}
