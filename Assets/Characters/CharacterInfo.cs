using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Characters/Character", order = 0)]
public class CharacterInfo : ScriptableObject
{
    public string characterName;
    public Sprite sprite;
    public Color defaultColor = Color.white;
    public GameObject prefab;
}
