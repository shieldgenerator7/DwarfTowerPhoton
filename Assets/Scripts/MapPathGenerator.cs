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
        //Initialize MapPath
        mapPath = new MapPath();
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
        generatePathSegment(buildDir);
        //Add middle points
        for (int i = 0; i < 5; i++)
        {
            buildDir = generateNewDirection(buildDir);
            generatePathSegment(buildDir);
        }
        //Add last point
        mapPath.addToStart(startPosition, true);
        //Delegate
        onMapPathGenerated?.Invoke(mapPath);
    }
    public delegate void OnMapPathGenerated(MapPath mapPath);
    public event OnMapPathGenerated onMapPathGenerated;

    private void generatePathSegment(Vector2 buildDir)
    {
        Vector2 newPos;
        do
        {
            float length = Random.Range(minSegmentLength, maxSegmentLength);
            newPos = mapPath.Start + (buildDir.normalized * length);
        }
        while (!withinBounds(newPos));
        mapPath.addToStart(newPos, true);
    }

    private bool withinBounds(Vector2 pos) => true || paddedBounds.Contains(pos);

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
