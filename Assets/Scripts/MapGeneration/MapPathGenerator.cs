using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPathGenerator : MonoBehaviour
{

    public MapPath mapPath { get; private set; }

    public void generateMapPath(MapProfile mapProfile)
    {
        //Initialize MapPath
        int safetyEject = 100;
        do
        {
            mapPath = mapProfile.caravanPathAlgorithm.generate(mapProfile.caravanPathReqs);
            safetyEject--;
            if (safetyEject == 0)
            {
                Debug.Log($"Safety eject! mapPath: {mapPath.Length}");
                break;
            }
        }
        while (!mapProfile.caravanPathReqs.validMapPath(mapPath));
        Debug.Log($"Generated map path. Length: {mapPath.Length}");
        //Delegate
        onMapPathGenerated?.Invoke(mapPath);
    }
    public delegate void OnMapPathGenerated(MapPath mapPath);
    public event OnMapPathGenerated onMapPathGenerated;






}
