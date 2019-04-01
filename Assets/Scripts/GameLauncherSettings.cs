using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLauncherSettings : MonoBehaviour
{
    [Range(1,10)]
    public int clientCount = 4;
    public bool resetClients = true;
}
