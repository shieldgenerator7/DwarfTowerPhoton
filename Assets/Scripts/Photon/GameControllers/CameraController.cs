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
    public Camera Cam { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Cam = Camera.main;
        if (focusObject != null)
        {
            FocusObject = focusObject;
        }
        playAreaBounds = FindObjectOfType<MapGenerator>().mapProfile.VisibleBounds;
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
            Cam.ViewportToWorldPoint(Vector2.one)
            - Cam.ViewportToWorldPoint(Vector2.zero);
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

    /// <summary>
    /// Returns whether or not the given position is in the camera's view
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsInView(Vector2 position, float buffer = 0)
    {
        //2021-12-12: copied from Stonicorn.CameraController.inView()
        //2017-10-31: copied from an answer by Taylor-Libonati: http://answers.unity3d.com/questions/720447/if-game-object-is-in-cameras-field-of-view.html
        Vector3 screenPoint = Cam.WorldToViewportPoint(position);
        return screenPoint.x > 0 + buffer && screenPoint.x < 1 - buffer
            && screenPoint.y > 0 + buffer && screenPoint.y < 1 - buffer;
    }
    public Rect ViewRect
        //TODO: make this more efficient
        => new Rect(
            Cam.ViewportToWorldPoint(new Vector2(0, 0)),
            Cam.ViewportToWorldPoint(new Vector2(1, 1))
                - Cam.ViewportToWorldPoint(new Vector2(0, 0))
            );
}
