using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MapPath : IEnumerable
{
    private List<Vector2> points;

    public Vector2 Start
    {
        get => points[0];
        set => points[0] = value;
    }
    public Vector2 End
    {
        get => points[points.Count - 1];
        set => points[points.Count - 1] = value;
    }

    public float Length { get; private set; }

    public Vector2 Middle { get; private set; }

    public MapPath() : this(Vector2.zero) { }
    public MapPath(Vector2 initialMiddle)
    {
        points = new List<Vector2>();
        points.Add(initialMiddle);
    }
    public MapPath(Vector2 initialStart, Vector2 initialEnd)
    {
        points = new List<Vector2>();
        points.Add(initialStart);
        points.Add(initialEnd);
    }

    /// <summary>
    /// Adds the given position to the start
    /// </summary>
    /// <param name="pos">The position to add</param>
    /// <param name="balanced">Should it also add a symmetrically equivalent position to the end?</param>
    public void addToStart(Vector2 pos, bool balanced = true)
    {
        if (balanced)
        {
            Vector2 middle = Middle;
            Vector2 endPos = middle + (middle - pos);
            points.Add(endPos);
        }
        points.Insert(0, pos);
        updateInfoVariables();
    }
    private void updateInfoVariables()
    {
        //Length
        float length = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {
            length += Vector2.Distance(points[i], points[i + 1]);
        }
        Length = length;
        //Middle
        Middle = getPosition(Length / 2);
    }

    /// <summary>
    /// Gets the position along the path at the given distance from the start position
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public Vector2 getPosition(float distance)
    {
        distance = Mathf.Clamp(distance, 0, Length);
        if (points.Count == 1)
        {
            return points[0];
        }
        float lengthSoFar = 0;
        for (int i = 1; i < points.Count; i++)
        {
            Vector2 point = points[i];
            Vector2 prevPoint = points[i - 1];
            lengthSoFar += Vector2.Distance(prevPoint, point);
            if (lengthSoFar == distance)
            {
                //This point is at the distance
                return point;
            }
            else if (lengthSoFar > distance)
            {
                //The distance is between point and prevPoint
                float diff = lengthSoFar - distance;
                Vector2 dir = (prevPoint - point).normalized * diff;
                return point + dir;
            }
        }
        throw new UnityException("Something went wrong calculating position from distance!");
    }

    public Vector2 getPositionFromPercentage(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 1);
        return getPosition(Length * percentage);
    }

    /// <summary>
    /// Returns the minimum distance between the given point and this path
    /// </summary>
    /// <param name="point"></param>
    /// <param name="minimum"></param>
    /// <returns></returns>
    public float distanceFromPath(Vector2 point, float minimum = 0)
    {
        float minDistance = float.MaxValue;
        for (int i = 1; i < points.Count; i++)
        {
            minDistance = Mathf.Min(
                minDistance,
                Utility.distanceToSegment(point, points[i - 1], points[i])
                );
            //Return if found minimum early
            if (minDistance <= minimum)
            {
                return minimum;
            }
        }
        return minDistance;
    }

    public IEnumerator GetEnumerator() => points.GetEnumerator();

    public static implicit operator List<Vector2>(MapPath mp)
        => mp.points.ToList();
}
