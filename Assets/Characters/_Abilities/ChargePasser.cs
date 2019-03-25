using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargePasser : ChargedShotController
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ObjectSpawner>().onObjectSpawned += passCharge;
    }

    void passCharge(GameObject go)
    {
        go.GetComponent<ChargedShotController>().chargeStats(multiplier);
    }
}
