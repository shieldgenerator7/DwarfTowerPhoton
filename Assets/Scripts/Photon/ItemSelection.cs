using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSelection<T>
{
    private int index = -1;
    public int Index
    {
        get => index;
        set
        {
            index = Mathf.Clamp(value, -1, itemList.Count);
            onIndexChanged?.Invoke(index);
        }
    }
    public delegate void OnIndexChanged(int index);
    public event OnIndexChanged onIndexChanged;

    public int SelectedIndex => Mathf.Max(index, 0);

    public T SelectedItem
    {
        get => (index >= 0)
            ? itemList[index]
            : itemList[0];
        set
        {
            Index = itemList.IndexOf(value);
        }
    }
    public List<T> itemList;

    public T this[int i]
    {
        get => itemList[i];
        set => itemList[i] = value;
    }

    public bool Contains(T item) => itemList.Contains(item);

    public int IndexOf(T item) => itemList.IndexOf(item);
}
