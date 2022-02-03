using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuDisplay : MenuDisplay
{
    public float minWidth = 100;
    [Header("Character Display")]
    public Image characterImage;
    [Header("Character Info")]
    public TMP_Text characterTypeText;
    public List<Image> difficultyStars;
    public Sprite starFull;
    public Sprite starEmpty;
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
        playerInfo.characterSelection.onIndexChanged += updateCharacterImage;
        playerInfo.SelectRandomCharacter();
        updateCharacterImage(playerInfo.characterSelection.Index);
        playerInfo.onColorChanged += updateColorImage;
        updateColorImage(playerInfo.DefaultColor);
    }
    private void OnDestroy()
    {
        PlayerInfo playerInfo = PlayerInfo.instance;
        playerInfo.characterSelection.onIndexChanged -= updateCharacterImage;
        playerInfo.onColorChanged -= updateColorImage;
    }

    void updateCharacterImage(int index)
    {
        PlayerInfo playerInfo = PlayerInfo.instance;
        CharacterInfo charInfo = playerInfo.characterSelection[index];
        characterImage.sprite = charInfo.portrait;
        characterImage.color = playerInfo.DefaultColor;
        RectTransform rect = characterImage.GetComponent<RectTransform>();
        rect.sizeDelta = 320 * charInfo.portrait.rect.size / 16;
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

    void updateColorImage(Color color)
    {
        characterImage.color = color;
    }
}
