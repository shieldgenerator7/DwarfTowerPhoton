
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
#if UNITY_EDITOR
    //public float Width => width;
    [SerializeField]
    [Range(10, 100)]
    [Tooltip("The width of the play area [TEST ONLY]")]
    public float width;

    //public float Height => Height;
    [SerializeField]
    [Range(10, 100)]
    [Tooltip("The height of the play area [TEST ONLY]")]
    public float height;
#endif

    public SpriteRenderer ground;
    public SpriteRenderer left;
    public SpriteRenderer right;
    public SpriteRenderer top;
    public SpriteRenderer bottom;

    public void adjustPlayArea(float width, float height)
    {
        Debug.Log($"width: {width}, height: {height}");
        //Field
        ground.size = new Vector2(width, height);
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
}
