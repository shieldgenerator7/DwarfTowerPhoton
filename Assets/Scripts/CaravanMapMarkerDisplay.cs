using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanMapMarkerDisplay : MonoBehaviour
{
    public SpriteRenderer positiveArrow;
    public SpriteRenderer positiveArrowOutline;
    public SpriteRenderer negativeArrow;
    public SpriteRenderer negativeArrowOutline;
    private List<SpriteRenderer> allSRs;

    private CaravanController caravanController;

    public void Init(CaravanController caravanController)
    {
        this.caravanController = caravanController;
        allSRs = gameObject.FindComponents<SpriteRenderer>(true);
        //Register delegate for showing/hiding
        MapMarker mapMarker = gameObject.FindComponent<MapMarker>();
        if (mapMarker)
        {
            mapMarker.onShow +=
                (show) =>
                {
                    allSRs.ForEach(sr => sr.enabled = show);
                    this.enabled = show;
                };
        }
    }

    private void Update()
    {
        float dir = caravanController.direction.y;
        //Debug.Log($"Caravan Dir: {caravanController.direction}");
        positiveArrow.enabled = dir > 0;
        negativeArrow.enabled = dir < 0;
        positiveArrowOutline.enabled = dir > 0;
        negativeArrowOutline.enabled = dir < 0;
        //Update color
        if (caravanController.winningTeamCaptain)
        {
            positiveArrow.color = caravanController.winningTeamCaptain.teamColor;
            negativeArrow.color = caravanController.winningTeamCaptain.teamColor;
        }
    }
}
