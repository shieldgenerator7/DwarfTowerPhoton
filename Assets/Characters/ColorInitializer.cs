using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorInitializer : MonoBehaviour
{
    public void setColor(Color color)
    {
        gameObject.FindComponent<PhotonView>().RPC(
            "RPC_SetColor",
            RpcTarget.All,
            (int)PlayerInfo.instance.allColors.IndexOf(color)
            );
    }

    [PunRPC]
    void RPC_SetColor(int index)
    {
        Color color = PlayerInfo.instance.allColors[index];
        gameObject.FindComponents<SpriteRenderer>()
            .ForEach(sr => sr.color = color);
        gameObject.FindComponents<ObjectSpawner>()
            .ForEach(os => os.PlayerColor = color);
        gameObject.FindComponents<ObjectAutoSpawner>()
            .ForEach(oas => oas.PlayerColor = color);
        //Destroy this script, bc it is one-time use on start up only
        Destroy(this);
    }
}
