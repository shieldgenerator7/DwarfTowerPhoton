using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotController : MonoBehaviour
{
    //
    // Settings
    //
    public float speed = 3;//how fast it travels (units/sec)
    public float damagePerSecond = 10;//damage per second in seconds
    public float stunDuration = 5;//how long hit players will be stunned for
    public float knockbackDistance = 10;//how far (in total) hit players will be knocked back
    public float maxHealth = 1;//how much health this shot has

    public bool Stuns
    {
        get { return stunDuration > 0; }
    }
    public bool Knocksback
    {
        get { return knockbackDistance > 0; }
    }
    public bool HitsPlayer
    {
        get { return Stuns || Knocksback; }
    }
    public bool HitsObjects
    {
        get { return damagePerSecond > 0; }
    }

    //
    // Runtime Vars
    //

    [SerializeField]
    private float health;
    protected float Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    //
    // Components
    //

    private Rigidbody2D rb2d;
    public GunController parentGC;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = transform.up * speed;
        health = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        processCollision(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        processCollision(collision);
    }

    void processCollision(Collider2D collision) { 
        if (HitsObjects)
        {
            ShotController sc = collision.gameObject.GetComponent<ShotController>();
            if (sc)
            {
                sc.addHealth(-damagePerSecond * Time.deltaTime);
            }
        }
        if (HitsPlayer)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void addHealth(float health)
    {
        this.Health += health;
    }
}
