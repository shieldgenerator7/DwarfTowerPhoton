using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorInitializer : MonoBehaviour
{
    public void setColor(Color color)
    {
        var colorIndex = PlayerInfo.instance.getColorIndex(color);
        gameObject.FindComponent<PhotonView>().RPC(
            "RPC_SetColor",
            RpcTarget.All,
            colorIndex.groupIndex,
            colorIndex.colorIndex
            );
    }

    [PunRPC]
    void RPC_SetColor(int groupIndex, int colorIndex)
    {
        Color color = PlayerInfo.instance.colorGroups[groupIndex][colorIndex];
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
