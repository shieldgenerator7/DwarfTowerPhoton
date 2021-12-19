using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=QbSI3Ajscgc

    public static PlayerInfo instance;

    [Tooltip("The name of the map that the player wants to go to")]
    public string mapName;

    [SerializeField]
    private int mySelectedCharacter;
    public int SelectedIndex
    {
        get => mySelectedCharacter;
        set
        {
            mySelectedCharacter = Mathf.Clamp(value, 0, allCharacters.Count - 1);
            onSelectedIndexChanged?.Invoke(mySelectedCharacter);
        }
    }
    public delegate void OnSelectedIndexChanged(int index);
    public event OnSelectedIndexChanged onSelectedIndexChanged;

    public CharacterInfo SelectedCharacter
    {
        get => allCharacters[mySelectedCharacter];
        set
        {
            SelectedIndex = allCharacters.IndexOf(value);
        }
    }

    public List<CharacterInfo> allCharacters;
    public List<CharacterInfo> unlockedCharacters;
    public List<CharacterInfo> unlockableAtStart;
    public int unlockAtStartCount = 2;

    private int colorIndex = -1;
    public int ColorIndex
    {
        get => (colorIndex >= 0)
            ? colorIndex
            : allColors.IndexOf(SelectedCharacter.defaultColor);
        set
        {
            colorIndex = Mathf.Clamp(value, -1, allColors.Count);
            if (colorIndex >= 0)
            {
                if (colorPreferences.Contains(colorIndex))
                {
                    colorPreferences.Remove(colorIndex);
                }
                colorPreferences.Insert(0, colorIndex);
            }
            onSelectedColorChanged?.Invoke(ColorIndex);
        }
    }
    public event OnSelectedIndexChanged onSelectedColorChanged;
    private List<int> colorPreferences = new List<int>();

    public Color SelectedColor
    {
        get => (colorIndex >= 0)
            ? allColors[colorIndex]
            : SelectedCharacter.defaultColor;
        set
        {
            ColorIndex = allColors.IndexOf(value);
        }
    }
    public List<Color> allColors;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
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

    public int getUniqueColorIndex()
    {
        //Get list of all other players
        List<PlayerController> players = FindObjectsOfType<PlayerController>().ToList();
        //Our player has not been made yet, so no need to remove it from the list
        //players.RemoveAll(plyr => plyr.PV.IsMine);
        //Get list of all untaken colors
        List<int> untakenColors = new List<int>();
        for (int i = 0; i < allColors.Count; i++)
        {
            untakenColors.Add(i);
        }
        players.ForEach(
            plyr => untakenColors.Remove(allColors.IndexOf(plyr.playerColor))
            );
        //Select untaken color from SelectedColor
        int selectedIndex = ColorIndex;
        if (untakenColors.Contains(selectedIndex))
        {
            return selectedIndex;
        }
        //Select untaken color from preferences
        foreach (int colorIndex in colorPreferences)
        {
            if (untakenColors.Contains(colorIndex))
            {
                return colorIndex;
            }
        }
        //No preferences available, so choose random one
        int randIndex = Random.Range(0, untakenColors.Count);
        return untakenColors[randIndex];
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
        SelectedCharacter = unlockedCharacters[index];
    }
}
