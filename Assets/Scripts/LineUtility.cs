using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//2021-12-08: copied from https://pastebin.com/iQDhQTFN
public class LineEquation
{
    public LineEquation(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;

        A = End.y - Start.y;
        B = Start.x - End.x;
        C = A * Start.x + B * Start.y;
    }

    public Vector2 Start { get; private set; }
    public Vector2 End { get; private set; }

    public float A { get; private set; }
    public float B { get; private set; }
    public float C { get; private set; }

    public Vector2? GetIntersectionWithLine(LineEquation otherLine)
    {
        float determinant = A * otherLine.B - otherLine.A * B;

        if (determinant.IsZero()) //lines are parallel
            return default(Vector2?);

        //Cramer's Rule

        float x = (otherLine.B * C - B * otherLine.C) / determinant;
        float y = (A * otherLine.C - otherLine.A * C) / determinant;

        Vector2 intersectionVector2 = new Vector2(x, y);

        return intersectionVector2;
    }

    public Vector2? GetIntersectionWithLineSegment(LineEquation otherLine)
    {
        Vector2? intersectionVector2 = GetIntersectionWithLine(otherLine);

        if (intersectionVector2.HasValue &&
            intersectionVector2.Value.IsBetweenTwoVector2s(otherLine.Start, otherLine.End))
            return intersectionVector2;

        return default(Vector2?);
    }

    //i didnt review this one for correctness
    public LineEquation GetIntersectionWithLineForRay(Rect rectangle)
    {
        LineEquation intersectionLine;

        if (Start == End)
            return null;

        IEnumerable<LineEquation> lines = rectangle.LineSegments();
        intersectionLine = new LineEquation(new Vector2(0, 0), new Vector2(0, 0));
        var intersections = new Dictionary<LineEquation, Vector2>();
        foreach (LineEquation equation in lines)
        {
            Vector2? intersectionVector2 = GetIntersectionWithLineSegment(equation);

            if (intersectionVector2.HasValue)
                intersections[equation] = intersectionVector2.Value;
        }

        if (!intersections.Any())
            return null;

        var intersectionVector2s = new SortedDictionary<float, Vector2>();
        foreach (var intersection in intersections)
        {
            if (End.IsBetweenTwoVector2s(Start, intersection.Value) ||
                intersection.Value.IsBetweenTwoVector2s(Start, End))
            {
                float distanceToVector2 = Start.DistanceToVector2(intersection.Value);
                intersectionVector2s[distanceToVector2] = intersection.Value;
            }
        }

        if (intersectionVector2s.Count == 1)
        {
            Vector2 endVector2 = intersectionVector2s.First().Value;
            intersectionLine = new LineEquation(Start, endVector2);

            return intersectionLine;
        }

        if (intersectionVector2s.Count == 2)
        {
            Vector2 start = intersectionVector2s.First().Value;
            Vector2 end = intersectionVector2s.Last().Value;
            intersectionLine = new LineEquation(start, end);

            return intersectionLine;
        }

        return null;
    }

    public override string ToString()
        => $"({Start})], [{End}]";
}

public static class DoubleExtensions
{
    //SOURCE: https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/Precision.cs
    //        https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/Precision.Equality.cs
    //        http://referencesource.microsoft.com/#WindowsBase/Shared/MS/Internal/DoubleUtil.cs
    //        http://stackoverflow.com/questions/2411392/float-epsilon-for-equality-greater-than-less-than-less-than-or-equal-to-gre

    /// <summary>
    /// The smallest positive number that when SUBTRACTED from 1D yields a result different from 1D.
    /// The value is derived from 2^(-53) = 1.1102230246251565e-16, where IEEE 754 binary64 &quot;float precision&quot; floating point numbers have a significand precision that utilize 53 bits.
    ///
    /// This number has the following properties:
    ///     (1 - NegativeMachineEpsilon) &lt; 1 and
    ///     (1 + NegativeMachineEpsilon) == 1
    /// </summary>
    public const float NegativeMachineEpsilon = 1.1102230246251565e-16f; //Math.Pow(2, -53);

    /// <summary>
    /// The smallest positive number that when ADDED to 1D yields a result different from 1D.
    /// The value is derived from 2 * 2^(-53) = 2.2204460492503131e-16, where IEEE 754 binary64 &quot;float precision&quot; floating point numbers have a significand precision that utilize 53 bits.
    /// 
    /// This number has the following properties:
    ///     (1 - PositiveDoublePrecision) &lt; 1 and
    ///     (1 + PositiveDoublePrecision) &gt; 1
    /// </summary>
    public const float PositiveMachineEpsilon = 2f * NegativeMachineEpsilon;

    /// <summary>
    /// The smallest positive number that when SUBTRACTED from 1D yields a result different from 1D.
    /// 
    /// This number has the following properties:
    ///     (1 - NegativeMachineEpsilon) &lt; 1 and
    ///     (1 + NegativeMachineEpsilon) == 1
    /// </summary>
    public static readonly float MeasuredNegativeMachineEpsilon = MeasureNegativeMachineEpsilon();

    private static float MeasureNegativeMachineEpsilon()
    {
        float epsilon = 1f;

        do
        {
            float nextEpsilon = epsilon / 2f;

            if ((1D - nextEpsilon) == 1D) //if nextEpsilon is too small
                return epsilon;

            epsilon = nextEpsilon;
        }
        while (true);
    }

    /// <summary>
    /// The smallest positive number that when ADDED to 1D yields a result different from 1D.
    /// 
    /// This number has the following properties:
    ///     (1 - PositiveDoublePrecision) &lt; 1 and
    ///     (1 + PositiveDoublePrecision) &gt; 1
    /// </summary>
    public static readonly float MeasuredPositiveMachineEpsilon = MeasurePositiveMachineEpsilon();

    private static float MeasurePositiveMachineEpsilon()
    {
        float epsilon = 1f;

        do
        {
            float nextEpsilon = epsilon / 2f;

            if ((1f + nextEpsilon) == 1f) //if nextEpsilon is too small
                return epsilon;

            epsilon = nextEpsilon;
        }
        while (true);
    }

    const float DefaultDoubleAccuracy = NegativeMachineEpsilon * 10f;

    public static bool IsClose(this float value1, float value2)
    {
        return IsClose(value1, value2, DefaultDoubleAccuracy);
    }

    public static bool IsClose(this float value1, float value2, float maximumAbsoluteError)
    {
        if (float.IsInfinity(value1) || float.IsInfinity(value2))
            return value1 == value2;

        if (float.IsNaN(value1) || float.IsNaN(value2))
            return false;

        float delta = value1 - value2;

        //return Math.Abs(delta) <= maximumAbsoluteError;

        if (delta > maximumAbsoluteError ||
            delta < -maximumAbsoluteError)
            return false;

        return true;
    }

    public static bool LessThan(this float value1, float value2)
    {
        return (value1 < value2) && !IsClose(value1, value2);
    }

    public static bool GreaterThan(this float value1, float value2)
    {
        return (value1 > value2) && !IsClose(value1, value2);
    }

    public static bool LessThanOrClose(this float value1, float value2)
    {
        return (value1 < value2) || IsClose(value1, value2);
    }

    public static bool GreaterThanOrClose(this float value1, float value2)
    {
        return (value1 > value2) || IsClose(value1, value2);
    }

    public static bool IsOne(this float value)
    {
        float delta = value - 1f;

        //return Math.Abs(delta) <= PositiveMachineEpsilon;

        if (delta > PositiveMachineEpsilon ||
            delta < -PositiveMachineEpsilon)
            return false;

        return true;
    }

    public static bool IsZero(this float value)
    {
        //return Math.Abs(value) <= PositiveMachineEpsilon;

        if (value > PositiveMachineEpsilon ||
            value < -PositiveMachineEpsilon)
            return false;

        return true;
    }
}

public static class Vector2Extensions
{
    public static float DistanceToVector2(this Vector2 point, Vector2 point2)
    {
        return Mathf.Sqrt((point2.x - point.x) * (point2.x - point.x) + (point2.y - point.y) * (point2.y - point.y));
    }

    public static float SquaredDistanceToVector2(this Vector2 point, Vector2 point2)
    {
        return (point2.x - point.x) * (point2.x - point.x) + (point2.y - point.y) * (point2.y - point.y);
    }

    public static bool IsBetweenTwoVector2s(this Vector2 targetVector2, Vector2 point1, Vector2 point2)
    {
        float minX = Mathf.Min(point1.x, point2.x);
        float minY = Mathf.Min(point1.y, point2.y);
        float maxX = Mathf.Max(point1.x, point2.x);
        float maxY = Mathf.Max(point1.y, point2.y);

        float targetX = targetVector2.x;
        float targetY = targetVector2.y;

        return minX.LessThanOrClose(targetX)
              && targetX.LessThanOrClose(maxX)
              && minY.LessThanOrClose(targetY)
              && targetY.LessThanOrClose(maxY);
    }
}

public static class RectExtentions
{
    //improved name from original
    public static IEnumerable<LineEquation> LineSegments(this Rect rectangle)
    {
        var lines = new List<LineEquation>
            {
                new LineEquation(new Vector2(rectangle.x, rectangle.y),
                                 new Vector2(rectangle.x, rectangle.y + rectangle.height)),

                new LineEquation(new Vector2(rectangle.x, rectangle.y + rectangle.height),
                                 new Vector2(rectangle.x + rectangle.width, rectangle.y + rectangle.height)),

                new LineEquation(new Vector2(rectangle.x + rectangle.width, rectangle.y + rectangle.height),
                                 new Vector2(rectangle.x + rectangle.width, rectangle.y)),

                new LineEquation(new Vector2(rectangle.x + rectangle.width, rectangle.y),
                                 new Vector2(rectangle.x, rectangle.y)),
            };

        return lines;
    }
}
