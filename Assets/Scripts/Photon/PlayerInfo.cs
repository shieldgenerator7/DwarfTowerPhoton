using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=QbSI3Ajscgc

    public static PlayerInfo instance;

    [Tooltip("The name of the map that the player wants to go to")]
    public string mapName;

    public ItemSelection<CharacterInfo> characterSelection;

    public List<CharacterInfo> unlockedCharacters;
    public List<CharacterInfo> unlockableAtStart;
    public int unlockAtStartCount = 2;

    public ItemSelection<Color> warmColorSelection;
    public ItemSelection<Color> coolColorSelection;

    public List<ItemSelection<Color>> colorGroups;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            initColorGroups();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Populate unlocked characters
        while (unlockedCharacters.Count < this.unlockAtStartCount)
        {
            int index = Random.Range(0, unlockableAtStart.Count);
            CharacterInfo character = unlockableAtStart[index];
            if (!unlockedCharacters.Contains(character))
            {
                unlockedCharacters.Add(character);
            }
        }
        //if (PlayerPrefs.HasKey("MyCharacter"))
        //{
        //    mySelectedCharacter = PlayerPrefs.GetInt("MyCharacter");
        //}
        //else
        //{
        //    mySelectedCharacter = 0;
        //    PlayerPrefs.SetInt("MyCharacter", mySelectedCharacter);
        //}
    }

    /// <summary>
    /// This is here so it works well with the TextMeshPro InputField
    /// </summary>
    /// <param name="mapName"></param>
    public void setMapName(string mapName)
    {
        this.mapName = mapName;
    }

    public void SelectRandomCharacter()
    {
        int index = Random.Range(0, unlockedCharacters.Count);
        characterSelection.SelectedItem = unlockedCharacters[index];
    }

    #region Color Groups

    private void initColorGroups()
    {
        //Color groups
        colorGroups = new List<ItemSelection<Color>>()
        {
            warmColorSelection,
            coolColorSelection
        };
        //Register Delegates
        for (int i = 0; i < colorGroups.Count; i++)
        {
            colorGroups[i].onIndexChanged += (index) =>
            {
                onColorChanged?.Invoke(colorGroups[i][index]);
            };
        }
    }
    public delegate void OnColorChanged(Color color);
    public event OnColorChanged onColorChanged;

    public (int groupIndex, int colorIndex) getColorIndex(Color color)
    {
        for (int i = 0; i < colorGroups.Count; i++)
        {
            if (colorGroups[i].Contains(color))
            {
                return (i, colorGroups[i].IndexOf(color));
            }
        }
        return (-1, -1);
    }

    public Color DefaultColor
    {
        get
        {
            foreach (ItemSelection<Color> colorGroup in colorGroups)
            {
                if (colorGroup.Index >= 0)
                {
                    return colorGroup.SelectedItem;
                }
                else
                {

                }
            }
            return characterSelection.SelectedItem.defaultColor;
        }
    }

    #endregion
}
