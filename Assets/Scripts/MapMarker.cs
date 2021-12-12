using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarker : MonoBehaviour
{
    private Vector2 targetPosition;
    private Transform followObj;
    private MapMarkerInfo info;
    private TeamToken placer;
    //[Tooltip("Should this marker show up on the map even when its position is in the camera view?")]
    //public bool showWhenOnScreen = false;

    [Header("Components")]
    public SpriteRenderer iconSR;
    public SpriteRenderer markerSR;

    public void Init(MapMarkerInfo info, TeamToken placer)
    {
        Init(
            info,
            placer,
            placer?.teamCaptain.teamColor ?? Color.white,
            placer?.teamCaptain.teamColor ?? Color.white
            );
    }

    public void Init(MapMarkerInfo info, Color iconColor, Color markerColor)
    {
        Init(info, null, iconColor, markerColor);
    }

    public void Init(MapMarkerInfo info, TeamToken placer, Color iconColor, Color markerColor)
    {
        this.info = info;
        this.placer = placer;
        iconSR.sprite = info.icon;
        iconSR.color = iconColor;
        markerSR.color = markerColor;
    }

    public void Mark(Vector2 pos)
    {
        if (info.canMarkPosition)
        {
            targetPosition = pos;
        }
        else
        {
            Debug.LogError($"This map marker {info.name} should not be able to mark position!");
        }
    }

    public void Mark(Transform followObject)
    {
        if (info.canFollowObject)
        {
            this.followObj = followObject;
            targetPosition = followObj.transform.position;
        }
        else
        {
            Debug.LogError($"This map marker {info.name} should not be able to follow an object!");
        }
    }

    private CameraController camCtr;

    // Start is called before the first frame update
    void Start()
    {
        camCtr = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followObj)
        {
            targetPosition = followObj.position;
        }
        updatePositionOnScreen();
    }

    void updatePositionOnScreen()
    {
        if (camCtr.IsInView(targetPosition))
        {
            //Position
            transform.position = targetPosition;
            //Rotation
            transform.up = Vector2.up;
        }
        else
        {
            Vector2 pointDir = targetPosition - (Vector2)camCtr.transform.position;
            //Position
            transform.position = Utility.rayIntersectRectangle(pointDir, camCtr.ViewRect);
            //Rotation
            transform.up = -pointDir;
        }
        //Make sure icon is always the right way up
        iconSR.transform.up = Vector2.up;
    }


}
