using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPathDisplayer : MonoBehaviour
{
    public Sprite pathSprite;
    public Color pathColor = Color.white;
    public GameObject lineSegmentPrefab;
    public float extrudeSegmentEnds = 0.5f;
    public MapPathGenerator generator;

    private List<GameObject> segments = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        generator.onMapPathGenerated += displayMapPath;
        if (generator.mapPath != null)
        {
            displayMapPath(generator.mapPath);
        }
    }

    void displayMapPath(MapPath mapPath)
    {
        //Destroy old map path segments
        for (int i = 0; i < segments.Count; i++)
        {
            Destroy(segments[i]);
        }
        segments.Clear();
        //Add new map path segments
        Vector2 prevPos = Vector2.positiveInfinity;
        foreach (Vector2 pos in mapPath)
        {
            //Don't process first one
            if (prevPos != Vector2.positiveInfinity)
            {
                //Process all the other ones
                Vector2 start = prevPos;
                Vector2 end = pos;
                Vector2 dir = (end - start).normalized;
                Vector2 extrudeV = dir * extrudeSegmentEnds;
                start -= extrudeV;
                end += extrudeV;
                GameObject segment = Instantiate(lineSegmentPrefab);
                segment.transform.parent = this.transform;
                SpriteRenderer sr = segment.GetComponent<SpriteRenderer>();
                sr.sprite = pathSprite;
                sr.color = pathColor;
                segment.transform.position = start;
                Vector2 size = sr.size;
                size.y = Vector2.Distance(start, end);
                sr.size = size;
                segment.transform.up = dir;
                segments.Add(segment);
            }
            prevPos = pos;
        }
    }
}
