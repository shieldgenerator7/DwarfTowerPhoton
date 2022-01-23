using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitFlash : MonoBehaviour
{
    private float flashDuration = 0.1f;
    private Color flashColor = Color.red;

    private float flashStartTime = -1;
    private List<SpriteRenderer> srs;
    private List<Color> origColors;

    // Start is called before the first frame update
    void Start()
    {
        srs = gameObject.FindComponents<SpriteRenderer>();
        origColors = srs.ConvertAll(sr => sr.color);
        this.flashDuration = EffectManager.instance.flashDuration;
        this.flashColor = EffectManager.instance.flashColor;
        gameObject.FindComponent<HealthPool>().onDamaged += startFlash;
    }

    // Update is called once per frame
    void Update()
    {
        if (flashStartTime >= 0 && Time.time >= flashStartTime + flashDuration)
        {
            endFlash();
        }
    }

    void startFlash(float hp)
    {
        cleanSrs();
        flashStartTime = Time.time;
        srs.ForEach(sr => sr.color = flashColor);
        enabled = true;
    }

    void endFlash()
    {
        cleanSrs();
        flashStartTime = -1;
        for (int i = 0; i < srs.Count; i++)
        {
            srs[i].color = origColors[i];
        }
        enabled = false;
    }

    void cleanSrs()
    {
        srs.RemoveAll(sr => sr == null);
    }
}
