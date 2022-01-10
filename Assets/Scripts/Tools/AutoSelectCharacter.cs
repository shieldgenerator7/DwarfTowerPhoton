using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSelectCharacter : MonoBehaviour
{
    [Range(0, 4)]
    public int characterIndex;
    [Range(0, 3)]
    public int warmColorIndex;
    [Range(0, 3)]
    public int coolColorIndex;
    public string mapName;

    // Start is called before the first frame update
    void Awake()
    {
        //Do once before
        AutoSelect();
    }
    private void Start()
    {
        //And do once after
        AutoSelect();
    }
    private void AutoSelect()
    {
        PlayerInfo info = FindObjectOfType<PlayerInfo>();
        info.characterSelection.Index = characterIndex;
        info.warmColorSelection.Index = warmColorIndex;
        info.coolColorSelection.Index = coolColorIndex;
        info.mapName = mapName;
    }
}
