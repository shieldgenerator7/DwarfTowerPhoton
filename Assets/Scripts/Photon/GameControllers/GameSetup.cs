using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=JjfPaY57dDM

    public static GameSetup instance;

    public Transform[] spawnPoints;

    private void OnEnable()
    {
        if (GameSetup.instance == null)
        {
            GameSetup.instance = this;
        }
    }
}
