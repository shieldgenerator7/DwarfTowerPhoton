using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewDisplayer : MonoBehaviour
{
    public Color buildColor = Color.white;//shows that object will be built
    public Color invalidColor = Color.gray;//shows that no action will be taken if finalized at this point
    public Color destroyColor = Color.red;//shows that another object will be destroyed
    public Color upgradeColor = Color.green;//shows that another object will be upgraded

    public enum PreviewState
    {
        BUILD,
        NONE,
        DESTROY,
        UPGRADE
    }

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

}
