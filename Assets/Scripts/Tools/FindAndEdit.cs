#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FindAndEdit : MonoBehaviour
{
    [Header("Search Settings")]
    public Component findComponent;
    public Component notThereComponent;
    public Component addComponent;
    public Component removeComponent;
    [Header("Search Results")]
    public List<Component> foundComponents;
    public List<Component> addedComponents;
    public List<GameObject> removedComponentsGameObjects;

    public List<Component> FindMonoBehaviours()
    {
        //Find all objects that have the find component
        foundComponents = AssetDatabase.FindAssets("t:prefab").ToList()
            .ConvertAll(guiID => AssetDatabase.LoadAssetAtPath<GameObject>(
                AssetDatabase.GUIDToAssetPath(guiID)
                )
            )
            .ConvertAll(go => go.GetComponent(findComponent.GetType()));
        //Remove null components
        foundComponents.RemoveAll(mb => mb == null);
        //Remove objects that have the notThereComponent
        if (notThereComponent)
        {
            foundComponents.RemoveAll(mb => mb.GetComponent(notThereComponent.GetType()));
        }
        //Remove this gameobject
        foundComponents.RemoveAll(mb => mb.gameObject == gameObject);
        //Return found components
        return foundComponents;
    }

    public void SelectComponents()
    {
        Selection.objects = foundComponents.ToArray();
    }

    public void SelectGameObjects()
    {
        Selection.objects = foundComponents
            .ConvertAll(comp => comp.gameObject).ToArray();
    }

    public void AddComponent()
    {
        Type type = addComponent.GetType();
        addedComponents = foundComponents
            .FindAll(mb => !mb.gameObject.GetComponent(type))
            .ConvertAll(mb => (Component)mb.gameObject.AddComponent(type));
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

    public void ClearSettings()
    {
        //Search settings
        DestroyImmediate(findComponent, true);
        findComponent = null;
        DestroyImmediate(notThereComponent, true);
        notThereComponent = null;
        DestroyImmediate(addComponent, true);
        addComponent = null;
        DestroyImmediate(removeComponent, true);
        removeComponent = null;
        //Search results
        foundComponents.Clear();
        addedComponents.Clear();
        removedComponentsGameObjects.Clear();
    }
}
#endif
