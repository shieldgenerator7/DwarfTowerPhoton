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
    public List<CharacterType> typeList;

    public string typeString
    {
        get
        {
            string types = "";
            //Add all types to string
            foreach(CharacterType charType in typeList)
            {
                types += charType + ", ";
            }
            //Trim off ending comma
            types = types.Trim();
            types = types.Substring(0, types.Length - 1);
            //Return
            return types;
        }
    }
}
