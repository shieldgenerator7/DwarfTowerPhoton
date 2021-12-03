
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    [Range(10, 100)]
    [Tooltip("The width of the play area")]
    public float width;
    [Range(10, 100)]
    [Tooltip("The height of the play area")]
    public float height;

    public Bounds Bounds =>
        new Bounds(Vector2.zero, new Vector2(width + 1, height + 1));

#if UNITY_EDITOR

    public SpriteRenderer grass;
    public SpriteRenderer left;
    public SpriteRenderer right;
    public SpriteRenderer top;
    public SpriteRenderer bottom;

    public void adjustPlayArea()
    {
        Debug.Log($"width: {width}, height: {height}");
        //Field
        grass.size = new Vector2(width, height);
        //Border size
        Vector2 szHorizontal = new Vector2(width + 1, 1);
        Vector2 szVertical = new Vector2(1, height + 1);
        left.size = right.size = szVertical;
        top.size = bottom.size = szHorizontal;
        //Border collider size
        left.GetComponent<BoxCollider2D>().size = szVertical;
        right.GetComponent<BoxCollider2D>().size = szVertical;
        top.GetComponent<BoxCollider2D>().size = szHorizontal;
        bottom.GetComponent<BoxCollider2D>().size = szHorizontal;
        //Border position
        left.transform.position = new Vector2(-width / 2, 0);
        right.transform.position = new Vector2(width / 2, 0);
        top.transform.position = new Vector2(0, height / 2);
        bottom.transform.position = new Vector2(0, -height / 2);
    }
#endif
}
