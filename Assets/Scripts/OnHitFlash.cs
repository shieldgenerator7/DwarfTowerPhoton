using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitFlash : MonoBehaviour
{
    [Tooltip("How many seconds the flash lasts")]
    public float flashDuration = 0.1f;
    [Tooltip("The color it flashes")]
    public Color flashColor = Color.red;

    private float flashStartTime = -1;
    private List<SpriteRenderer> srs;
    private List<Color> origColors;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.FindComponent<HealthPool>().onDamaged += startFlash;
        srs = gameObject.FindComponents<SpriteRenderer>();
        origColors = srs.ConvertAll(sr => sr.color);
    }

    // Update is called once per frame
    void Update()
    {
        if (flashStartTime >= 0 && Time.time >= flashStartTime + flashDuration)
        {
            endFlash();
        }
    }

    void startFlash()
    {
        flashStartTime = Time.time;
        srs.ForEach(sr => sr.color = flashColor);
        enabled = true;
    }

    void endFlash()
    {
        flashStartTime = -1;
        for(int i = 0; i < srs.Count; i++)
        {
            srs[i].color = origColors[i];
        }
        enabled = false;
    }
}
