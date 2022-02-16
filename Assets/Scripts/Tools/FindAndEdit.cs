#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindAndEdit : MonoBehaviour
{
    public MonoBehaviour findComponent;
    public List<MonoBehaviour> foundComponents;

    public List<MonoBehaviour> FindMonoBehaviours()
    {
        foundComponents = Resources.FindObjectsOfTypeAll(findComponent.GetType())
            .ToList()
            .ConvertAll(obj => (MonoBehaviour)obj);
        foundComponents.RemoveAll(mb => mb.gameObject == gameObject);
        return foundComponents;
    }

}
#endif
