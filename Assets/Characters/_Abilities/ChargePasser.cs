using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargePasser : ChargedShotController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        GetComponent<ObjectAutoSpawner>().onObjectSpawned += passCharge;
    }

    void passCharge(GameObject go)
    {
        Debug.Log($"passcharge on go: {go.name }, {this.gameObject.name}");
        go.GetComponent<ChargedShotController>().chargeStats(multiplier);
    }
}
