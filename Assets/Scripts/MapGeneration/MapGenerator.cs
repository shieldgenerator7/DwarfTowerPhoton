using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MapProfile mapProfile;

    [Header("Components")]
    public PlayArea playArea;
    public MapPathGenerator caravanPathGenerator;
    public ObstaclePopulator obstaclePopulator;

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
        //Initialize random seed
        if (seed <= 0)
        {
            string mapName = PlayerInfo.instance.mapName;
            if (!string.IsNullOrEmpty(mapName))
            {
                seed = mapName.GetHashCode();
            }
            else
            {
                seed = (int)System.DateTime.Now.Ticks;
            }
        }
        Random.InitState(seed);
        //Init MapProfile
        mapProfile.init();
        //Adjust Play Area
        playArea.ground.sprite = mapProfile.groundSprite;
        playArea.ground.color = mapProfile.groundColor;
        playArea.adjustPlayArea(mapProfile.playAreaSize.x, mapProfile.playAreaSize.y);
        //Caravan Path
        if (generateCaravanPath)
        {
            caravanPathGenerator.generateMapPath(mapProfile);
        }
        //Obstacles
        if (generateObstacles)
        {
            obstaclePopulator.populateObstacles(mapProfile);
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
