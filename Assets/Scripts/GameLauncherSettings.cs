using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameLauncherSettings : MonoBehaviour
{
    [Range(1, 10)]
    public int clientCount = 4;
    public bool resetClients = true;

    public List<Process> buildProcesses = new List<Process>();
}
