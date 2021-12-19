using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagMapMarkerDisplay : MonoBehaviour
{
    public List<SpriteRenderer> artifactSlots;
    private List<SpriteRenderer> allSRs;

    public void Init(FlagController flagController)
    {
        allSRs = gameObject.FindComponents<SpriteRenderer>(true);
        //Register delegates for updating artifact sprites
        for (int i = 0; i < flagController.pedestalList.Count; i++)
        {
            Pedestal pdstl = flagController.pedestalList[i];
            SpriteRenderer slot = artifactSlots[i];
            pdstl.onArtifactChanged +=
               (artifact) => UpdateArtifactSlot(slot, artifact);
            UpdateArtifactSlot(slot, null);// pdstl.Artifact);
        }
        //Register delegate for showing/hiding
        gameObject.FindComponent<MapMarker>().onShow +=
            (show) => allSRs.ForEach(sr => sr.enabled = show);
    }

    private void UpdateArtifactSlot(SpriteRenderer slot, Artifact artifact)
    {
        if (artifact != null)
        {
            slot.enabled = true;
            SpriteRenderer artSR = artifact.gameObject.FindComponent<SpriteRenderer>();
            slot.sprite = artSR.sprite;
            slot.color = artSR.color;
        }
        else
        {
            slot.enabled = false;
            slot.sprite = null;
            slot.color = Color.white;
        }
    }
}
