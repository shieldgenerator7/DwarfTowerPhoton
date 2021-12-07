using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPathGenerator : MonoBehaviour
{
    public PathGenerationRequirements pathGenerationRequirements;
    public MapPath mapPath;
    [Tooltip("How far inside the play bounds it must stay")]
    public float boundPadding = 5;

    [Header("Components")]
    public PlayArea playArea;
    private Bounds playBounds;
    private Bounds paddedBounds;
    public MapPathGeneratorAlgorithm mapPathGeneratorAlgorithm;


    // Start is called before the first frame update
    void Start()
    {
        //Initialize bounds
        playBounds = playArea.Bounds;
        paddedBounds = playBounds;
        Vector2 size = paddedBounds.size;
        size.x -= boundPadding * 2;
        size.y -= boundPadding * 2;
        paddedBounds.size = size;
        pathGenerationRequirements.bounds = paddedBounds;
        generateMapPath();
    }

    public void generateMapPath()
    {
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
