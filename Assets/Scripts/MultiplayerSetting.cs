using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSetting : MonoBehaviour
{//2019-03-14: made from following this tutorial: https://www.youtube.com/watch?v=IsiWRD1Xh5g

    public static MultiplayerSetting multiplayerSetting;

    public bool delayStart;
    public int maxPlayers;

    public int menuScene;
    public int multiplayerScene;

    private void Awake()
    {
        if (multiplayerSetting == null)
        {
            multiplayerSetting = this;
        }
        else
        {
            if (multiplayerSetting != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

}
