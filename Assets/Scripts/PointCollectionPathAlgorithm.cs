using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PointCollectionAlgorithm", menuName = "Map Generation/Path/Point Collection", order = 1)]
public class PointCollectionPathAlgorithm : MapPathGeneratorAlgorithm
{
    [Range(0, 10)]
    [Tooltip("How many points to collect. The number of points will be ((pointCount * 2) + 2)")]
    public int pointCount = 5;
    [Range(0, 10)]
    [Tooltip("Points should have x and y values that differ by at least this amount. " +
        "This is to prevent overlapping parallel path segments")]
    public float minPointDiff = 1;
    [Range(0, 10)]
    [Tooltip("The y buffer to apply to the area between the startPos and the middle")]
    public float bufferY = 5;
    [Tooltip("Points that generated points should not spawn aligned with, either horizontally or vertically")]
    public List<Vector2> avoidPointList;

    public override MapPath generate(PathGenerationRequirements pgp)
    {
        //Set generation bounds
        Bounds bounds = pgp.bounds;
        bounds.min = new Vector2(
            bounds.min.x,
            pgp.startPos.y + bufferY
            );
        bounds.max = new Vector2(
            bounds.max.x,
            pgp.middle.y - bufferY
            );
        //Generate points to "collect"
        List<Vector2> pointList = new List<Vector2>();
        for (int i = 0; i < pointCount; i++)
        {
            int safetyEject = 100;
            Vector2 point;
            do
            {
                point = new Vector2(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y)
                    );
                safetyEject--;
                if (safetyEject == 0)
                {
                    Debug.Log($"Safety eject! point: {point}");
                    break;
                }
            }
            while (pointList.Any(
                p => Mathf.Abs(p.x - point.x) < minPointDiff || Mathf.Abs(p.y - point.y) < minPointDiff
                )
            || avoidPointList.Any(
                p => Mathf.Abs(p.x - point.x) < minPointDiff || Mathf.Abs(p.y - point.y) < minPointDiff
                )
            );
            //Point is going to cause overlapping parallel path segments, so add it
            //(or safety eject was used, in which case add it anyway)
            pointList.Add(point);
        }
        //Add start position to points to collect
        pointList.Add(pgp.startPos);
        //"Collect" the points with the path
        MapPath mapPath = new MapPath(pgp.middle);
        foreach (Vector2 point in pointList)
        {
            //Move up/down to point
            mapPath.addToStart(
                new Vector2(
                    mapPath.Start.x,
                    point.y
                    ),
                true
                );
            //Move left/right to point
            mapPath.addToStart(
                new Vector2(
                    point.x,
                    mapPath.Start.y
                    ),
                true
                );
            //ok, point collected, move on to next point
        }
        //Ok, all points collected, return path
        return mapPath;
    }
}
