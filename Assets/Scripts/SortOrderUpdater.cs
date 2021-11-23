using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class SortOrderUpdater : MonoBehaviour
{
    [Tooltip("Should this update the sorting order every frame? If false, only sets it on start")]
    public bool updateEveryFrame = true;

    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.updateSortingOrder();
        if (!updateEveryFrame)
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        sr.updateSortingOrder();
    }
}
