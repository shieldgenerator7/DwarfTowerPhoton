using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallGrass : MonoBehaviour
{
    [Range(0, 1)]
    [Tooltip("The alpha value of player sprites when inside the tall grass")]
    public float hiddenAlpha = 0.5f;

    [Tooltip("The type of objects this tall grass can hide")]
    public List<EntityType> hideableTypes;

    private Collider2D coll2d;

    private void Start()
    {
        coll2d = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        checkSetAlpha(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        checkSetAlpha(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        checkSetAlpha(collision.gameObject);
    }

    void checkSetAlpha(GameObject go)
    {
        HealthPool hp = go.GetComponent<HealthPool>();
        if (hp && hideableTypes.Contains(hp.entityType))
        {
            if (coll2d.OverlapPoint(go.transform.position))
            {
                setAlpha(go, hiddenAlpha);
            }
            else
            {
                setAlpha(go, 1);
            }
        }
    }

    void setAlpha(GameObject go, float alpha)
    {
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        sr.color = sr.color.setAlpha(alpha);
    }
}
