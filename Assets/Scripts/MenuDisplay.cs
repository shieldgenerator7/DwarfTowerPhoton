using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDisplay : MonoBehaviour
{
    [Header("Character Select")]
    public Image characterImage;
    public Text characterTypeText;
    public Image characterImageBackground;
    public GridLayoutGroup characterSelectGroup;
    public GameObject characterSelectButtonPrefab;
    public GridLayoutGroup colorSelectGroup;
    public GameObject colorSelectButtonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInfo playerInfo = PlayerInfo.instance;
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
        List<CharacterInfo> allCharacters = PlayerInfo.instance.allCharacters;
        //Create character select buttons
        foreach (CharacterInfo charInfo in allCharacters)
        {
            GameObject btnCharSel = Instantiate(characterSelectButtonPrefab);
            btnCharSel.transform.parent = characterSelectGroup.transform;
            Image img = btnCharSel.GetComponent<Image>();
            img.sprite = charInfo.sprite;
            img.color = charInfo.defaultColor;
            Text txt = btnCharSel.GetComponentInChildren<Text>();
            txt.text = charInfo.characterName.ToUpper();
            Button btn = btnCharSel.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                PlayerInfo.instance.SelectedCharacter = charInfo;
            });
        }
        //Resize grid layout
        RectTransform rect = characterSelectGroup.GetComponent<RectTransform>();
        int charCount = allCharacters.Count;
        int width = charCount * 100 + (charCount + 1) * 10;
        rect.sizeDelta = new Vector2(
            width,
            rect.sizeDelta.y
            );
        //Resize background
        RectTransform rectBG = characterImageBackground.GetComponent<RectTransform>();
        rectBG.sizeDelta = new Vector2(
            width,
            rectBG.sizeDelta.y
            );
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
        characterTypeText.text = charInfo.typeString;
    }

    void updateColorImage(int index)
    {
        characterImage.color = PlayerInfo.instance.allColors[index];
    }
}
