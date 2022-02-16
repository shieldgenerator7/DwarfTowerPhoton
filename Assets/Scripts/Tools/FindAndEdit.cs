#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindAndEdit : MonoBehaviour
{
    [Header("Search Settings")]
    public MonoBehaviour findComponent;
    public MonoBehaviour addComponent;
    public MonoBehaviour removeComponent;
    [Header("Search Results")]
    public List<MonoBehaviour> foundComponents;
    public List<MonoBehaviour> addedComponents;
    public List<GameObject> removedComponentsGameObjects;

    public List<MonoBehaviour> FindMonoBehaviours()
    {
        foundComponents = Resources.FindObjectsOfTypeAll(findComponent.GetType())
            .ToList()
            .ConvertAll(obj => (MonoBehaviour)obj);
        foundComponents.RemoveAll(mb => mb.gameObject == gameObject);
        return foundComponents;
    }

    public void AddComponent()
    {
        Type type = addComponent.GetType();
        addedComponents = foundComponents
            .FindAll(mb => !mb.gameObject.GetComponent(type))
            .ConvertAll(mb => (MonoBehaviour)mb.gameObject.AddComponent(type));
    }

    public void RemoveComponent()
    {
        Type type = removeComponent.GetType();
        removedComponentsGameObjects = foundComponents
            .FindAll(mb => mb.gameObject.GetComponent(type))
            .ConvertAll(mb => mb.gameObject);
        removedComponentsGameObjects
            .ForEach(go => DestroyImmediate(go.GetComponent(type), true));
    }
}
#endif
