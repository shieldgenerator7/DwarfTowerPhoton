using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPathGenerator : MonoBehaviour
{
    public PathGenerationRequirements pathGenerationRequirements;

    [Header("Components")]
    public MapPathGeneratorAlgorithm mapPathGeneratorAlgorithm;

    public MapPath mapPath { get; private set; }

    public void generateMapPath(Bounds generatableBounds)
    {
        pathGenerationRequirements.bounds = generatableBounds;
        //Initialize MapPath
        int safetyEject = 100;
        do
        {
            mapPath = mapPathGeneratorAlgorithm.generate(pathGenerationRequirements);
            safetyEject--;
            if (safetyEject == 0)
            {
                Debug.Log($"Safety eject! mapPath: {mapPath.Length}");
                break;
            }
        }
        while (!validMapPath(mapPath));
        Debug.Log($"Generated map path. Length: {mapPath.Length}");
        //Delegate
        onMapPathGenerated?.Invoke(mapPath);
    }
    public delegate void OnMapPathGenerated(MapPath mapPath);
    public event OnMapPathGenerated onMapPathGenerated;


    private bool validMapPath(MapPath mapPath)
    {
        float length = mapPath.Length;
        return length >= pathGenerationRequirements.minLength
            && length <= pathGenerationRequirements.maxLength;
    }



}
