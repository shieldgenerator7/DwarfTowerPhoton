using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PreviewDisplayer : MonoBehaviour
{
    public Color buildColor = Color.white;//shows that object will be built
    public Color invalidColor = Color.gray;//shows that no action will be taken if finalized at this point
    public Color destroyColor = Color.red;//shows that another object will be destroyed
    public Color upgradeColor = Color.green;//shows that another object will be upgraded

    public bool faceMousePointer = false;
    public enum PreviewState
    {
        BUILD,
        NONE,
        DESTROY,
        UPGRADE
    }

    private List<SpriteRenderer> srs = new List<SpriteRenderer>();
    private List<Sprite> sprites;

    private void Start()
    {
        //SpriteRenderers        
        srs.Add(GetComponent<SpriteRenderer>());
        srs.AddRange(GetComponentsInChildren<SpriteRenderer>());
        srs.RemoveAll(sr => sr == null);
        //Sprites
        sprites = srs.ConvertAll(sr => sr.sprite);
        //Face Mouse Pointer
        if (!faceMousePointer)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (faceMousePointer)
        {
            //Face mouse pointer
            Vector2 dir = ((Vector2)(Utility.MouseWorldPos - transform.position)).normalized;
            transform.up = dir;
        }
    }

    public void updatePreviewSprite(Sprite sprite = null)
    {
        if (sprite)
        {
            srs.ForEach(sr => sr.sprite = sprite);
        }
        else
        {
            for (int i = 0; i < srs.Count; i++)
            {
                srs[i].sprite = sprites[i];
            }
        }
    }

    public void updatePreviewColor(PreviewState state)
    {
        Color color = Color.white;
        switch (state)
        {
            case PreviewState.BUILD: color = buildColor; break;
            case PreviewState.NONE: color = invalidColor; break;
            case PreviewState.DESTROY: color = destroyColor; break;
            case PreviewState.UPGRADE: color = upgradeColor; break;
        }
        srs.ForEach(sr => sr.color = color);
    }

    public bool boundsIntersects(Bounds bounds)
    {
        return srs.Any(sr => sr.bounds.Intersects(bounds));
    }
}
