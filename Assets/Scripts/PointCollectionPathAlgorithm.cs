using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PointCollectionAlgorithm", menuName = "Map Generation/Path/Point Collection", order = 1)]
public class PointCollectionPathAlgorithm : MapPathGeneratorAlgorithm
{
    [Range(0, 10)]
    public int pointCount = 5;
    public float buffer = 5;

    public override MapPath generate(PathGenerationRequirements pgp)
    {
        //Set generation bounds
        Bounds bounds = pgp.bounds;
        bounds.min = new Vector2(
            bounds.min.x,
            pgp.startPos.y + buffer
            );
        bounds.max = new Vector2(
            bounds.max.x,
            pgp.middle.y - buffer
            );
        //Generate points to "collect"
        List<Vector2> pointList = new List<Vector2>();
        for (int i = 0; i < pointCount; i++)
        {
            Vector2 point = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
                );
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
