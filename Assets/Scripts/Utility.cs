using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{

    public static Vector3 MouseWorldPos
    {
        get { return Camera.main.ScreenToWorldPoint(Input.mousePosition); }
    }

    public static void teleportObject(GameObject go, Vector2 newPosition)
    {
        Rigidbody2D rb2d = go.FindComponent<Rigidbody2D>();
        rb2d.isKinematic = true;
        rb2d.transform.position = newPosition;
        rb2d.isKinematic = false;
    }

    public static T FindComponent<T>(this GameObject go, bool searchParent = true, bool searchChildren = true) where T : Component
    {
        T comp = go.GetComponent<T>();
        bool found = comp && comp.gameObject.name != "null";
        if (!found && searchParent)
        {
            comp = go.GetComponentInParent<T>();
            found = comp && comp.gameObject.name != "null";
        }
        if (!found && searchChildren)
        {
            comp = go.GetComponentInChildren<T>();
            found = comp && comp.gameObject.name != "null";
        }
        return comp;
    }

    public static bool isMoving(this Rigidbody2D rb2d)
    {
        return rb2d.velocity.sqrMagnitude > 0.1f;
        //return !Mathf.Approximately(rb2d.velocity.magnitude, 0);
    }

    public static void updateSortingOrder(this SpriteRenderer sr)
    {
        sr.sortingOrder = -(int)(sr.transform.position.y * 100);
    }
}
