using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMatrixComponent : MonoBehaviour
{
    public StatMatrix statMatrix;

    public StatLayer Stats => statMatrix.Stats;

    private void Start()
    {
        statMatrix.init();
    }

    public void triggerEvents()
    {
        statMatrix.triggerEvent();
    }
}
