using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject focusObject;//object the camera follows, usually the player
    private Vector3 offset;
    public GameObject FocusObject
    {
        get { return focusObject; }
        set
        {
            focusObject = value;
            float zOffset = transform.position.z - focusObject.transform.position.z;
            offset = new Vector3(0, 0, zOffset);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (focusObject != null)
        {
            FocusObject = focusObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (focusObject != null)
        {
            transform.position = focusObject.transform.position + offset;
        }
    }
}
