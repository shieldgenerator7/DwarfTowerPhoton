using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPathGenerator : MonoBehaviour
{
    public Vector2 startPosition;
    public Vector2 endPosition;
    public MapPath mapPath;

    [Header("Settings")]

    [Tooltip("Should all path segments be strictly horizontal or vertical?")]
    public bool forceRectangularPaths = true;
    public float minLength = 70;
    public float maxLength = 490;
    public float minSegmentLength = 1;
    public float maxSegmentLength = 20;
    [Tooltip("How far inside the play bounds it must stay")]
    public float boundPadding = 5;

    [Header("Components")]

    public PlayArea playArea;
    private Bounds playBounds;
    private Bounds paddedBounds;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize bounds
        playBounds = playArea.Bounds;
        paddedBounds = playBounds;
        Vector2 size = paddedBounds.size;
        size.x -= boundPadding * 2;
        size.y -= boundPadding * 2;
        paddedBounds.size = size;
        generateMapPath();
    }

    public void generateMapPath()
    {
        //Initialize MapPath
        mapPath = generateMapPath(Vector2.zero);
        //Delegate
        onMapPathGenerated?.Invoke(mapPath);
    }
    public delegate void OnMapPathGenerated(MapPath mapPath);
    public event OnMapPathGenerated onMapPathGenerated;

    private MapPath generateMapPath(Vector2 middle)
    {
        MapPath mapPath = new MapPath(middle);
        //Get build dir
        Vector2 buildDir = startPosition - endPosition;
        if (forceRectangularPaths)
        {
            if (Mathf.Abs(buildDir.x) > Mathf.Abs(buildDir.y))
            {
                //Make horizontal
                buildDir.y = 0;
            }
            else
            {
                //Make vertical
                buildDir.x = 0;
            }
        }
        //Add initial point
        Vector2 buildPos = generatePathPos(mapPath.Start, buildDir);
        mapPath.addToStart(buildPos, true);
        //Add middle points
        for (int i = 0; i < 6; i++)
        {
            buildDir = generateNewDirection(buildDir);
            buildPos = generatePathPos(mapPath.Start, buildDir);
            mapPath.addToStart(buildPos, true);
        }
        //Add second to last point
        mapPath.addToStart(new Vector2(startPosition.x, mapPath.Start.y), true);
        //Add last point
        mapPath.addToStart(startPosition, true);
        //Delegate
        onMapPathGenerated?.Invoke(mapPath);
    }
    public delegate void OnMapPathGenerated(MapPath mapPath);
    public event OnMapPathGenerated onMapPathGenerated;

    private Vector2 generatePathPos(Vector2 start, Vector2 buildDir)
    {
        float length = Random.Range(minSegmentLength, maxSegmentLength);
        return start + (buildDir.normalized * length);
    }

    private bool withinBounds(Vector2 pos) => paddedBounds.Contains(pos);

    private Vector2 generateNewDirection(Vector2 prevDirection)
    {
        if (forceRectangularPaths)
        {
            if (prevDirection.x != 0)
            {
                return new Vector2(
                    0,
                    prevDirection.x * ((Random.Range(0, 2) == 0) ? 1 : -1)
                    );
            }
            else
            {
                return new Vector2(
                    prevDirection.y * ((Random.Range(0, 2) == 0) ? 1 : -1),
                    0
                    );
            }
        }
        else
        {
            return Random.rotation.eulerAngles;
        }
    }
}
