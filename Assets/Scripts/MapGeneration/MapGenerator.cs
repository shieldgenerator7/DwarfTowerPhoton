using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MapProfile mapProfile { get; private set; }
    public List<MapProfile> mapProfiles;

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
    public List<Artifact> artifacts;

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
        mapProfile = mapProfiles[Random.Range(0, 100) % mapProfiles.Count];
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
            PlaceArtifacts(mapProfile);
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

    private void PlaceArtifacts(MapProfile mapProfile)
    {
        int countLeft = mapProfile.artifactCount;
        for (int i = artifacts.Count - 1; i >= 0; i--)
        {
            int index = Random.Range(0, artifacts.Count);
            Artifact artifact = artifacts[index];
            if (countLeft > 0)
            {
                countLeft--;
                artifact.transform.position = getRandomPosition(mapProfile.borderPadding);
            }
            else
            {
                PhotonNetwork.Destroy(artifact.gameObject);
            }
            artifacts.Remove(artifact);
        }
    }

    Vector2 getRandomPosition(float avoidRadius)
    {
        Vector2 min = mapProfile.GeneratableBounds.min;
        Vector2 max = mapProfile.GeneratableBounds.max;
        int safetyEject = 100;
        Vector2 pos = Vector2.zero;
        do
        {
            pos.x = Random.Range(min.x, max.x);
            pos.y = Random.Range(min.y, max.y);
            safetyEject--;
            if (safetyEject == 0)
            {
                Debug.Log($"Safety eject! avoidRadius: {avoidRadius}");
                break;
            }
        }
        while (obstaclePopulator.avoidPosList.Any(
            t => Vector2.Distance(t.position, pos) <= avoidRadius
            ));
        return pos;
    }
}
