using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public PlayArea playArea;
    public MapPathGenerator caravanPathGenerator;
    public ObstaclePopulator obstaclePopulator;

    [Tooltip("How far inside the play bounds it must stay")]
    public float boundPadding = 2;

    public Bounds generatableBounds { get; private set; }

    private PhotonView PV;

    // Start is called before the first frame update
    void Awake()
    {
        PV = gameObject.FindComponent<PhotonView>();
        if (PV.IsMine)
        {
            generateMap();
        }
    }

    public void generateMap(int seed = -1, bool generateCaravanPath = true, bool generateObstacles = true)
    {
        if (seed <= 0)
        {
            seed = (int)System.DateTime.Now.Ticks;
        }
        Random.InitState(seed);
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
        //RPC
        if (PV.IsMine)
        {
            PV.RPC("RPC_GenerateMap", RpcTarget.OthersBuffered, seed);
        }
    }

    [PunRPC]
    void RPC_GenerateMap(int seed)
    {
        generateMap(seed, true, false);
    }
}
