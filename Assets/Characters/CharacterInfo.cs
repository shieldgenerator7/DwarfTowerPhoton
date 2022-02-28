using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Characters/Character", order = 0)]
public class CharacterInfo : ScriptableObject
{
    public string characterName;
    public int characterID;
    public Sprite sprite;
    public Color defaultColor = Color.white;
    public Sprite portrait;
    public GameObject prefab;
    [Range(1,3)]
    [Tooltip("How difficult this character is perceived to be")]
    public int difficulty = 1;//the more stars, the more difficult
    [Tooltip("Roles that this character best fits into")]
    public List<CharacterType> typeList;

    public string typeString
    {
        get
        {
            string separator = " | ";// " \u2B24 ";
            string types = separator;
            //Add all types to string
            foreach(CharacterType charType in typeList)
            {
                types += charType + separator;
            }
            //Return
            return types;
        }
    }
}
