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
        if (rb2d)
        {
            rb2d.isKinematic = true;
            rb2d.transform.position = newPosition;
            rb2d.isKinematic = false;
        }
        else
        {
            go.transform.position = newPosition;
        }
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

    public static List<T> FindComponents<T>(this GameObject go, bool searchChildren = true) where T : Component
    {
        List<T> compList = new List<T>();
        compList.Add(go.GetComponent<T>());
        if (searchChildren)
        {
            compList.AddRange(go.GetComponentsInChildren<T>());
        }
        compList.RemoveAll(t => !t);
        return compList;
    }

    public static bool isPhotonViewMine(this Component comp)
    {
        PhotonView pv = comp.gameObject.FindComponent<PhotonView>();
        return pv.IsMine;
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

    public static Color setRGB(this Color color, Color rgb)
    {
        rgb.a = color.a;
        return rgb;
    }

    public static Color setAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public static float distanceToSegment(Vector2 point, Vector2 vertexA, Vector2 vertexB)
    {
        Vector2 closest = Vector2.zero;
        return distanceToSegment(point, vertexA, vertexB, out closest);
    }
    public static float distanceToSegment(Vector2 point, Vector2 vertexA, Vector2 vertexB, out Vector2 closest)
    {
        //2021-12-07: copied from http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/
        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        float dx = vertexB.x - vertexA.x;
        float dy = vertexB.y - vertexA.y;
        if ((dx == 0) && (dy == 0))
        {
            // It's a point not a line segment.
            closest = vertexA;
            dx = point.x - vertexA.x;
            dy = point.y - vertexA.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        // Calculate the t that minimizes the distance.
        float t = ((point.x - vertexA.x) * dx + (point.y - vertexA.y) * dy) /
            (dx * dx + dy * dy);

        // See if this represents one of the segment's
        // end points or a point in the middle.
        if (t < 0)
        {
            closest = new Vector2(vertexA.x, vertexA.y);
            dx = point.x - vertexA.x;
            dy = point.y - vertexA.y;
        }
        else if (t > 1)
        {
            closest = new Vector2(vertexB.x, vertexB.y);
            dx = point.x - vertexB.x;
            dy = point.y - vertexB.y;
        }
        else
        {
            closest = new Vector2(vertexA.x + t * dx, vertexA.y + t * dy);
            dx = point.x - closest.x;
            dy = point.y - closest.y;
        }
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

}
