using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuDisplay : MonoBehaviour
{
    public float minWidth = 100;
    [Header("Character Display")]
    public Image characterImage;
    [Header("Character Info")]
    public TMP_Text characterTypeText;
    public List<Image> difficultyStars;
    public Sprite starFull;
    public Sprite starEmpty;
    [Header("Character Select")]
    public GridLayoutGroup characterSelectGroup;
    public GameObject characterSelectButtonPrefab;
    [Header("Color Select")]
    public GridLayoutGroup colorSelectGroup;
    public GameObject colorSelectButtonPrefab;
    [Header("Map Name")]
    public TMP_InputField txtMapName;
    [Header("Other")]
    public List<RectTransform> elementsToWiden;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInfo playerInfo = PlayerInfo.instance;
        txtMapName.text = PlayerInfo.instance.mapName;
        txtMapName.onValueChanged.AddListener((mapName) => playerInfo.mapName = mapName);
        playerInfo.onSelectedIndexChanged += updateCharacterImage;
        playerInfo.SelectedIndex = Random.Range(0, playerInfo.allCharacters.Count);
        updateCharacterImage(playerInfo.SelectedIndex);
        playerInfo.onSelectedColorChanged += updateColorImage;
        updateColorImage(playerInfo.ColorIndex);
        populateCharacterSelect();
        populateColorSelect();
    }
    private void OnDestroy()
    {
        PlayerInfo playerInfo = PlayerInfo.instance;
        playerInfo.onSelectedIndexChanged -= updateCharacterImage;
        playerInfo.onSelectedColorChanged -= updateColorImage;
    }
    void populateCharacterSelect()
    {
        List<CharacterInfo> unlockedCharacters = PlayerInfo.instance.unlockedCharacters;
        //Create character select buttons
        foreach (CharacterInfo charInfo in unlockedCharacters)
        {
            GameObject btnCharSel = Instantiate(characterSelectButtonPrefab);
            btnCharSel.transform.parent = characterSelectGroup.transform;
            Image img = btnCharSel.FindComponent<Image>(false, true);
            img.sprite = charInfo.sprite;
            img.color = charInfo.defaultColor;
            RectTransform rect = img.GetComponent<RectTransform>();
            rect.sizeDelta = 100 * charInfo.sprite.rect.size / 16;
            TMP_Text txt = btnCharSel.GetComponentInChildren<TMP_Text>();
            txt.text = charInfo.characterName.ToUpper();
            Button btn = btnCharSel.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                PlayerInfo.instance.SelectedCharacter = charInfo;
            });
        }
        //Resize elements to widen
        int charCount = unlockedCharacters.Count;
        float width = charCount * 100 + (charCount + 1) * 10;
        width = Mathf.Max(width, minWidth);
        foreach (RectTransform rect in elementsToWiden)
        {
            rect.sizeDelta = new Vector2(
                width,
                rect.sizeDelta.y
                );
        }
    }

    void populateColorSelect()
    {
        List<Color> allColors = PlayerInfo.instance.allColors;
        //Create character select buttons
        foreach (Color color in allColors)
        {
            GameObject btnColorSel = Instantiate(colorSelectButtonPrefab);
            btnColorSel.transform.parent = colorSelectGroup.transform;
            Image img = btnColorSel.GetComponent<Image>();
            img.color = color;
            Button btn = btnColorSel.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                PlayerInfo.instance.SelectedColor = color;
            });
        }
        //Resize grid layout
        //RectTransform rect = colorSelectGroup.GetComponent<RectTransform>();
        //int colorCount = playerInfo.allColors.Count;
        //int width = colorCount * 100 + (colorCount + 1) * 10;
        //rect.sizeDelta = new Vector2(
        //    width,
        //    rect.sizeDelta.y
        //    );
    }

    void updateCharacterImage(int index)
    {
        PlayerInfo playerInfo = PlayerInfo.instance;
        CharacterInfo charInfo = playerInfo.allCharacters[index];
        characterImage.sprite = charInfo.sprite;
        characterImage.color = playerInfo.SelectedColor;
        //Type
        characterTypeText.text = charInfo.typeString;
        //Difficulty
        for (int i = 0; i < difficultyStars.Count; i++)
        {
            difficultyStars[i].sprite = (i < charInfo.difficulty)
                ? starFull
                : starEmpty;
        }
    }

    void updateColorImage(int index)
    {
        characterImage.color = PlayerInfo.instance.allColors[index];
    }
}
