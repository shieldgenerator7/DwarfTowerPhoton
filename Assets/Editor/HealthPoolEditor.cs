using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(HealthPool))]
public class HealthPoolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.enabled = !EditorApplication.isPlaying;
        //OnDieDestroy
        bool anyNeedsODD = targets.ToList().Any(t => !hasOnDieDestroy(t as HealthPool));
        if (anyNeedsODD)
        {
            if (GUILayout.Button("Destroy on die (Editor Only)"))
            {
                foreach (object t in targets)
                {
                    if (!hasOnDieDestroy(t as HealthPool))
                    {
                        addOnDieDestroy(t as HealthPool);
                    }
                }
            }
        }
        //Static Rigidbody2D
        bool anyNeedsRB2D = targets.ToList().Any(t => !hasRB2D(t as HealthPool));
        if (anyNeedsRB2D)
        {
            if (GUILayout.Button("Static RB2D (Editor Only)"))
            {
                foreach (object t in targets)
                {
                    if (!hasRB2D(t as HealthPool))
                    {
                        addStaticRB2D(t as HealthPool);
                    }
                }
            }
        }
    }

    bool hasOnDieDestroy(HealthPool hp) => hasComponent<OnDieDestroy>(hp);

    void addOnDieDestroy(HealthPool hp) => addComponent<OnDieDestroy>(hp);

    bool hasRB2D(HealthPool hp) => hasComponent<Rigidbody2D>(hp);

    void addStaticRB2D(HealthPool hp)
    {
        int indexOffset = (hasOnDieDestroy(hp)) ? 2 : 1;
        Rigidbody2D rb2d = addComponent<Rigidbody2D>(hp, indexOffset);
        //Make it static
        rb2d.bodyType = RigidbodyType2D.Static;
    }

    bool hasComponent<T>(HealthPool hp) where T : Component
    {
        T comp = hp.GetComponent<T>();
        return comp;
    }

    T addComponent<T>(HealthPool hp, int indexOffset = 1) where T : Component
    {
        T comp = hp.gameObject.AddComponent<T>();
        //Component reordering copied from:
        //2021-11-23: http://answers.unity.com/answers/1080266/view.html
        //2021-11-23: https://forum.unity.com/threads/component-index-guarantee.770501/#post-5130215
        List<Component> compList = hp.GetComponents<Component>().ToList();
        int hpIndex = compList.IndexOf(hp);
        int compIndex = compList.IndexOf(comp);
        while (compIndex > hpIndex + indexOffset)
        {
            UnityEditorInternal.ComponentUtility.MoveComponentUp(comp);
            compIndex = comp.GetComponents<Component>().ToList().IndexOf(comp);
        }
        UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(comp, false);
        return comp;
    }
}
