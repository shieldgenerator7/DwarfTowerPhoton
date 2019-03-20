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
}
