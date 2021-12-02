using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("On Hit Effect Settings")]
    [Tooltip("How many seconds the flash lasts")]
    public float flashDuration = 0.1f;
    [Tooltip("The color it flashes")]
    public Color flashColor = Color.red;

    public static EffectManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
