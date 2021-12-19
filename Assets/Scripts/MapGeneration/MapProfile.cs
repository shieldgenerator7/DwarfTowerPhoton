using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "MapProfile", menuName = "Map Generation/Map Profile", order = 1)]
public class MapProfile : ScriptableObject
{
    public Vector2 playAreaSize = new Vector2(40, 80);
    public Sprite groundSprite;
    public Color groundColor = Color.white;
    [Tooltip("How close to the border the objects are allowed to spawn")]
    public float borderPadding = 2;
    public PathGenerationRequirements caravanPathReqs;
    public MapPathGeneratorAlgorithm caravanPathAlgorithm;
    public int artifactCount = 4;
    public List<ObstacleInfo> obstacleList;

    public Bounds VisibleBounds { get; private set; }
    public Bounds PlayBounds { get; private set; }
    public Bounds GeneratableBounds { get; private set; }
    public void init()
    {
        //Initialize bounds
        VisibleBounds = new Bounds(
            Vector2.zero,
            new Vector2(
                playAreaSize.x + 1,
                playAreaSize.y + 1
                )
            );
        PlayBounds = new Bounds(
            Vector2.zero,
            new Vector2(
                playAreaSize.x - 1,
                playAreaSize.y - 1
                )
            );
        GeneratableBounds = new Bounds(
            Vector2.zero,
            new Vector2(
                playAreaSize.x - 1 - borderPadding * 2,
                playAreaSize.y - 1 - borderPadding * 2
                )
            );
        //Caravan Path
        caravanPathReqs.bounds = GeneratableBounds;
    }
}
