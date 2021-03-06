﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{

    public static Vector3 MouseWorldPos
    {
        get { return Camera.main.ScreenToWorldPoint(Input.mousePosition); }
    }

    public static IEnumerator teleportObject(GameObject go, Vector3 newPosition)
    {
        Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();
        rb2d.isKinematic = true;
        while (go.transform.position != newPosition)
        {
            go.transform.position = newPosition;
            yield return null;
        }
        rb2d.isKinematic = false;
        yield return null;
    }
}
