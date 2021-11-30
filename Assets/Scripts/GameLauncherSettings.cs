using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameLauncherSettings : MonoBehaviour
{
    [Range(1, 10)]
    public int clientCount = 4;
    public bool resetClients = true;
    public bool enterPlayMode = false;

    public List<int> buildProcesses = new List<int>();
}
