using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuDisplay : MenuDisplay
{
    [Header("Character Display")]
    public Image characterImage;
    public TMP_Text txtCharacterName;
    [Header("Map Name")]
    public TMP_InputField txtMapName;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInfo playerInfo = PlayerInfo.instance;
        txtMapName.text = PlayerInfo.instance.mapName;
        txtMapName.onValueChanged.AddListener((mapName) => playerInfo.mapName = mapName);
        playerInfo.characterSelection.onIndexChanged += updateCharacterImage;
        if (playerInfo.characterSelection.Index < 0)
        {
            playerInfo.SelectRandomCharacter();
        }
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
        txtCharacterName.text = charInfo.characterName.ToUpper();
    }

    void updateColorImage(Color color)
    {
        characterImage.color = color;
    }
}
