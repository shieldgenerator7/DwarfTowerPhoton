using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public PlayArea playArea;
    public MapPathGenerator caravanPathGenerator;
    public ObstaclePopulator obstaclePopulator;

    public bool generateCaravanPath = true;
    public bool generateObstacles = true;

    [Tooltip("How far inside the play bounds it must stay")]
    public float boundPadding = 2;

    public Bounds generatableBounds { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        if (this.isPhotonViewMine())
        {
            generateMap();
        }
    }

    public void generateMap()
    {
        //Initialize bounds
        Bounds bounds = playArea.PlayBounds;
        Vector2 size = bounds.size;
        size.x -= boundPadding * 2;
        size.y -= boundPadding * 2;
        bounds.size = size;
        generatableBounds = bounds;
        //Caravan Path
        if (generateCaravanPath)
        {
            caravanPathGenerator.generateMapPath(generatableBounds);
        }
        //Obstacles
        if (generateObstacles)
        {
            obstaclePopulator.populateObstacles(generatableBounds);
        }
    }
}
