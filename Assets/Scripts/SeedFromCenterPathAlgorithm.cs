using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    //fileName = "SeedFromCenterPathAlgorithm",
    menuName = "Map Generation/Path/Seed From Center", order = 1)]
public class SeedFromCenterPathAlgorithm : MapPathGeneratorAlgorithm
{

    [Range(3, 15)]
    public int segmentCount = 9;
    public float minSegmentLength = 1;
    public float maxSegmentLength = 20;
    public bool checkBounds = true;
    public bool checkFlagY = true;
    public bool checkLowerHalf = true;


    private MapPath mapPath;
    private PathGenerationRequirements pgp;

    public override MapPath generate(PathGenerationRequirements pgp)
    {
        this.pgp = pgp;
        mapPath = new MapPath(pgp.middle);
        //Get build dir
        Vector2 buildDir = pgp.startPos - pgp.endPos;
        if (pgp.forceRectangularPaths)
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
        Vector2 prevBuildDir = buildDir;
        for (int i = 0; i < segmentCount - 3; i++)
        {
            int safetyEject = 100;
            do
            {
                buildDir = generateNewDirection(prevBuildDir);
                buildPos = generatePathPos(mapPath.Start, buildDir);
                safetyEject--;
                if (safetyEject == 0)
                {
                    Debug.Log($"Safety eject! buildPos: {buildPos}");
                    break;
                }
            }
            while (!validPosition(buildPos));
            prevBuildDir = buildDir;
            mapPath.addToStart(buildPos, true);
        }
        //Add second to last point
        mapPath.addToStart(new Vector2(pgp.startPos.x, mapPath.Start.y), true);
        //Add last point
        mapPath.addToStart(pgp.startPos, true);
        //Return
        return mapPath;
    }
    private Vector2 generateNewDirection(Vector2 prevDirection)
    {
        if (pgp.forceRectangularPaths)
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
    private Vector2 generatePathPos(Vector2 start, Vector2 buildDir)
    {
        float length = Random.Range(minSegmentLength, maxSegmentLength);
        return start + (buildDir.normalized * length);
    }

    private bool validPosition(Vector2 pos)
        => true
        && (!checkBounds || withinBounds(pos))
        && (!checkFlagY || withinStartAndEndY(pos))
        && (!checkLowerHalf || withinLowerHalf(pos))
        ;
    private bool withinBounds(Vector2 pos)
        => pgp.bounds.Contains(pos);
    private bool withinStartAndEndY(Vector2 pos)
        => pos.y >= mapPath.Start.y
        && pos.y <= mapPath.End.y;
    private bool withinLowerHalf(Vector2 pos)
        => pos.y < 0;

}
