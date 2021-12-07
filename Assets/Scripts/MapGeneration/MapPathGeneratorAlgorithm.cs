using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapPathGeneratorAlgorithm : ScriptableObject
{
    public abstract MapPath generate(PathGenerationRequirements pgp);
}
