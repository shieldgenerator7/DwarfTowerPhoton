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

        bool anyNeedsODD = targets.ToList().Any(t => !hasOnDieDestroy((HealthPool)t));
        if (anyNeedsODD)
        {
            GUI.enabled = !EditorApplication.isPlaying;
            if (GUILayout.Button("Destroy on die"))
            {
                foreach (object t in targets)
                {
                    addOnDieDestroy((HealthPool)t);
                }
            }
        }
    }

    bool hasOnDieDestroy(HealthPool hp)
    {
        OnDieDestroy odd = hp.GetComponent<OnDieDestroy>();
        return odd;
    }

    void addOnDieDestroy(HealthPool hp)
    {
        OnDieDestroy odd = hp.GetComponent<OnDieDestroy>();
        if (!odd)
        {
            odd = hp.gameObject.AddComponent<OnDieDestroy>();
            //Component reordering copied from:
            //2021-11-23: http://answers.unity.com/answers/1080266/view.html
            //2021-11-23: https://forum.unity.com/threads/component-index-guarantee.770501/#post-5130215
            int hpIndex = hp.GetComponents<Component>().ToList().IndexOf(hp);
            int oddIndex = odd.GetComponents<Component>().ToList().IndexOf(odd);
            while (oddIndex > hpIndex + 1)
            {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(odd);
                oddIndex = odd.GetComponents<Component>().ToList().IndexOf(odd);
            }
        }
    }
}
