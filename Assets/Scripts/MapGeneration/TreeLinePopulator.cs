#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TreeLinePopulator : MonoBehaviour
{
    public Transform marker1;
    public float buffer1 = 0;
    public Transform marker2;
    public float buffer2 = 0;

    public GameObject treePrefab;
    public Transform folder;

    public List<float> percentages;
    public List<GameObject> trees;

    [Range(0, 1)]
    public float newPercent = 0;

    private Vector2 center;

    public void populate()
    {
        trees.ForEach(tree => DestroyImmediate(tree));
        trees.Clear();
        center = (marker1.position + marker2.position) / 2;
        Vector2 dir1 = ((Vector2)marker1.position - center);
        Vector2 dir2 = ((Vector2)marker2.position - center);
        foreach (float percent in percentages)
        {
            //Tree 1
            Vector2 pos1 = center + (dir1 * percent);
            populateTree(pos1, buffer1);
            //Tree 2
            Vector2 pos2 = center + (dir2 * percent);
            populateTree(pos2, buffer2);
        }
        EditorUtility.SetDirty(this);
    }
    void populateTree(Vector2 pos, float buffer)
    {
        GameObject tree = (GameObject)PrefabUtility.InstantiatePrefab(
            treePrefab, 
            gameObject.scene
            );
        setTreePosition(tree, pos + (Vector2.up * buffer));
        trees.Add(tree);
        tree.transform.parent = folder;
    }

    public void adjustLastTrees()
    {
        //Tree 1
        GameObject tree1 = trees[trees.Count - 2];
        adjustTree(tree1, marker1, buffer1);
        //Tree 2
        GameObject tree2 = trees[trees.Count - 1];
        adjustTree(tree2, marker2, buffer2);
    }

    void adjustTree(GameObject tree, Transform marker, float buffer)
    {
        Vector2 dir1 = ((Vector2)marker.position - center);
        Vector2 pos1 = center + (dir1 * percentages[percentages.Count - 1]);
        setTreePosition(tree, pos1 + (Vector2.up * buffer));
        EditorUtility.SetDirty(tree);
    }

    void setTreePosition(GameObject tree, Vector2 pos)
    {
        tree.transform.position = pos;
        tree.GetComponentsInChildren<SpriteRenderer>().ToList()
            .ForEach(sr => sr.updateSortingOrder());
    }
}
#endif
