using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperController : PlayerController
{
    [Tooltip("The ability that launches tall grass")]
    public GunController tallGrassLauncher;

    [System.Serializable]
    public struct TallGrassMap
    {
        public MapProfile mapProfile;
        public int objectSpawnIndex;
    }

    [Tooltip("This maps a MapProfile to the index in the object spawner that it should use")]
    public List<TallGrassMap> tallGrassMap;

    private LaserGunController laser;

    protected override void Start()
    {
        base.Start();

        laser = gameObject.FindComponent<LaserGunController>();

        //Tall Grass
        MapProfile mapProfile = FindObjectOfType<MapGenerator>().mapProfile;
        TallGrassMap mapping  = tallGrassMap.Find(tgm => tgm.mapProfile == mapProfile);
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
