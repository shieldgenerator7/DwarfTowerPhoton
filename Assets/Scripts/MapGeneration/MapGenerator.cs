using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MapProfile mapProfile;

    public string mapName
    {
        get => _mapName;
        private set
        {
            _mapName = value;
            onMapNameChanged?.Invoke(_mapName);
        }
    }
    public delegate void OnMapNameChanged(string mapName);
    public event OnMapNameChanged onMapNameChanged;
    private string _mapName = "";

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
            mapName = PlayerInfo.instance.mapName;
            if (string.IsNullOrEmpty(mapName))
            {
                mapName = $"{(int)System.DateTime.Now.Ticks}";
                mapName = mapName.Substring(mapName.Length - 5);
            }
            generateMap(mapName);
            //RPC
            PV.RPC("RPC_GenerateMap", RpcTarget.OthersBuffered, mapName);
        }
    }

    public void generateMap(string mapName, bool generateCaravanPath = true, bool generateObstacles = true)
    {
        //Initialize random seed
        this.mapName = mapName;
        int seed = mapName.GetHashCode();
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
        onMapGenerated?.Invoke(mapProfile);
    }
    public delegate void OnMapGenerated(MapProfile mapProfile);
    public event OnMapGenerated onMapGenerated;

    [PunRPC]
    void RPC_GenerateMap(string mapName)
    {
        generateMap(mapName, true, false);
    }
}
