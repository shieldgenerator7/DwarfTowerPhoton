using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSelectCharacter : MonoBehaviour
{
    [Range(0, 4)]
    public int characterIndex;
    [Range(0, 5)]
    public int colorIndex;
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
        info.SelectedIndex = characterIndex;
        info.ColorIndex = colorIndex;
        info.mapName = mapName;
    }
}
