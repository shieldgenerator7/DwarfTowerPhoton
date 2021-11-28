using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=QbSI3Ajscgc

    public static PlayerInfo instance;

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
        //Check default colors
        foreach (CharacterInfo charInfo in allCharacters)
        {
            if (!allColors.Contains(charInfo.defaultColor))
            {
                allColors.Add(charInfo.defaultColor);
                Debug.LogError(
                    "defaultColor not in the list! adding color: "
                    + ColorUtility.ToHtmlStringRGB(charInfo.defaultColor)
                    );
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
}
