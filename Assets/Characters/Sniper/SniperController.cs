using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperController : PlayerController
{
    [SerializeField]
    [Tooltip("The ability that shoots the laser")]
    private LaserGunController laser;
    [SerializeField]
    [Tooltip("The ability that launches tall grass")]
    private GunController tallGrassLauncher;

    [System.Serializable]
    public struct TallGrassMap
    {
        public MapProfile mapProfile;
        public int objectSpawnIndex;
    }

    [Tooltip("This maps a MapProfile to the index in the object spawner that it should use")]
    public List<TallGrassMap> tallGrassMap;


    protected override void InitializeSettings()
    {
        base.InitializeSettings();

        //Tall Grass
        MapProfile mapProfile = FindObjectOfType<MapGenerator>().mapProfile;
        TallGrassMap mapping = tallGrassMap.Find(tgm => tgm.mapProfile == mapProfile);
        tallGrassLauncher.shotIndex = mapping.objectSpawnIndex;
    }

    protected override void onAminaEmpty(float amina)
    {
        if (laser.Active)
        {
            onProcessingFinished -= reload;
            onProcessingFinished += reload;
        }
        else
        {
            reload();
        }
    }

    void reload()
    {
        onProcessingFinished -= reload;
        base.onAminaEmpty(aminaPool.Amina);
    }
}
