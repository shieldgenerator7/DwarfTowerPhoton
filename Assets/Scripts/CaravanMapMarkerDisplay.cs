using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanMapMarkerDisplay : MonoBehaviour
{
    public SpriteRenderer positiveArrow;
    public SpriteRenderer negativeArrow;
    private List<SpriteRenderer> allSRs;

    private CaravanController caravanController;

    public void Init(CaravanController caravanController)
    {
        this.caravanController = caravanController;
        allSRs = gameObject.FindComponents<SpriteRenderer>(true);
        //Register delegate for showing/hiding
        gameObject.FindComponent<MapMarker>().onShow +=
            (show) =>
            {
                allSRs.ForEach(sr => sr.enabled = show);
                this.enabled = show;
            };
    }

    private void Update()
    {
        float dir = caravanController.direction.y;
        //Debug.Log($"Caravan Dir: {caravanController.direction}");
        positiveArrow.enabled = dir > 0;
        negativeArrow.enabled = dir < 0;
    }
}
