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

    private Bounds playAreaBounds;

    // Start is called before the first frame update
    void Start()
    {
        if (focusObject != null)
        {
            FocusObject = focusObject;
        }
        playAreaBounds = FindObjectOfType<PlayArea>().VisibleBounds;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (focusObject != null)
        {
            transform.position = focusObject.transform.position + offset;
            boundCameraPosition();
        }
    }

    void boundCameraPosition()
    {
        //2021-12-03: copied from http://answers.unity.com/answers/1719329/view.html
        //Convert screen sizes to world coordinates
        Vector2 cameraSizeWorld =
            Camera.main.ViewportToWorldPoint(Vector2.one)
            - Camera.main.ViewportToWorldPoint(Vector2.zero);
        Vector2 halfSize = cameraSizeWorld / 2;
        //Use camera size to keep it in the bounds
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(
            pos.x,
            playAreaBounds.min.x + halfSize.x,
            playAreaBounds.max.x - halfSize.x
            );
        pos.y = Mathf.Clamp(
            pos.y,
            playAreaBounds.min.y + halfSize.y,
            playAreaBounds.max.y - halfSize.y
            );
        transform.position = pos;
    }
}
