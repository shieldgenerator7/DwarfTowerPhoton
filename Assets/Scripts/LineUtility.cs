using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//2021-12-08: copied from https://pastebin.com/iQDhQTFN
public struct LineEquation
{
    public LineEquation(Vector2 start, Vector2 end)
    {
        if (start == end)
        {
            throw new System.ArgumentException($"Start ({start}) and End ({end}) are not allowed to be the same value!");
        }

        Start = start;
        End = end;

        A = End.y - Start.y;
        B = Start.x - End.x;
        C = A * Start.x + B * Start.y;
    }

    public readonly Vector2 Start;
    public readonly Vector2 End;

    public readonly float A;
    public readonly float B;
    public readonly float C;

    public Vector2? GetIntersectionWithLine(LineEquation otherLine)
    {
        float determinant = A * otherLine.B - otherLine.A * B;

        //lines are parallel
        if (Mathf.Approximately(0, determinant))
        {
            return default(Vector2?);
        }

        //Cramer's Rule

        float x = (otherLine.B * C - B * otherLine.C) / determinant;
        float y = (A * otherLine.C - otherLine.A * C) / determinant;

        return new Vector2(x, y);
    }

    public Vector2? GetIntersectionWithRectangle(Rect rectangle)
    {
        //Check rect
        Rect checkRect = toRect();
        //Rect edges
        List<LineEquation> lines = rectangle.LineSegments();
        //Debug.Log($"Line x Rect: {lines[0]} :: {lines[1]} :: {lines[2]} :: {lines[3]}");
        foreach (LineEquation line in lines)
        {
            Vector2? intersectionQ = GetIntersectionWithLine(line);
            if (intersectionQ.HasValue)
            {
                Vector2 intersection = intersectionQ.Value;
                if (rectangle.ContainsInclusive(intersection, 0.01f)
                    && checkRect.ContainsInclusive(intersection, 0.01f))
                {
                    //Debug.Log($"Line x Rect: returning {intersection}. " +
                    //    $"line: {this}, rect: {rectangle}, " +
                    //    $"inside: {rectangle.ContainsInclusive(Start)}->{rectangle.ContainsInclusive(End)}"
                    //    );
                    return intersection;
                }
            }
        }

        Debug.Log($"Line x Rect: returning null. " +
                $"line: {this}, rect: {rectangle}, " +
                $"inside: {rectangle.ContainsInclusive(Start)}->{rectangle.ContainsInclusive(End)}"
                );

        return default(Vector2?);
    }

    public Rect toRect()
    {
        Rect rect = new Rect();
        rect.min = Vector2.Min(Start, End);
        rect.max = Vector2.Max(Start, End);
        return rect;
    }

    public override string ToString()
        => $"[{Start} -> {End}]";
}

public static class RectExtentions
{
    //improved name from original
    public static List<LineEquation> LineSegments(this Rect rectangle)
    {
        Vector2 bottomLeft = new Vector2(rectangle.min.x, rectangle.min.y);
        Vector2 topLeft = new Vector2(rectangle.min.x, rectangle.max.y);
        Vector2 topRight = new Vector2(rectangle.max.x, rectangle.max.y);
        Vector2 bottomRight = new Vector2(rectangle.max.x, rectangle.min.y);
        List<LineEquation> lines = new List<LineEquation>
        {
            new LineEquation(bottomLeft, topLeft),
            new LineEquation(topLeft, topRight),
            new LineEquation(topRight, bottomRight),
            new LineEquation(bottomRight, bottomLeft),
        };

        return lines;
    }

    public static bool ContainsInclusive(this Rect rect, Vector2 pos, float buffer = 0)
    {
        return rect.Contains(pos)
            || (pos.x >= rect.min.x - buffer && pos.x <= rect.max.x + buffer
            && pos.y >= rect.min.y - buffer && pos.y <= rect.max.y + buffer);
    }
}
