using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectMenuDisplay : MenuDisplay
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
    public List<GridLayoutGroup> colorSelectGroupList;
    public GameObject colorSelectButtonPrefab;
    [Header("Other")]
    public List<RectTransform> elementsToWiden;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInfo playerInfo = PlayerInfo.instance;
        playerInfo.characterSelection.onIndexChanged += updateCharacterImage;
        playerInfo.SelectRandomCharacter();
        updateCharacterImage(playerInfo.characterSelection.Index);
        playerInfo.onColorChanged += updateColorImage;
        updateColorImage(playerInfo.DefaultColor);
        populateCharacterSelect();
        populateColorSelect();
    }
    private void OnDestroy()
    {
        PlayerInfo playerInfo = PlayerInfo.instance;
        playerInfo.characterSelection.onIndexChanged -= updateCharacterImage;
        playerInfo.onColorChanged -= updateColorImage;
    }
    void populateCharacterSelect()
    {
        List<CharacterInfo> unlockedCharacters = PlayerInfo.instance.unlockedCharacters;
        //Create character select buttons
        foreach (CharacterInfo charInfo in unlockedCharacters)
        {
            GameObject btnCharSel = Instantiate(characterSelectButtonPrefab);
            btnCharSel.transform.parent = characterSelectGroup.transform;
            btnCharSel.transform.localScale = Vector3.one;
            Image img = btnCharSel.FindComponent<Image>(false, true);
            img.sprite = charInfo.portrait;
            img.color = charInfo.defaultColor;
            RectTransform rectImg = img.GetComponent<RectTransform>();
            rectImg.sizeDelta = 100 * charInfo.portrait.rect.size / 16;
            TMP_Text txt = btnCharSel.GetComponentInChildren<TMP_Text>();
            txt.text = charInfo.characterName.ToUpper();
            Button btn = btnCharSel.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                PlayerInfo.instance.characterSelection.SelectedItem = charInfo;
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
        for (int i = 0; i < PlayerInfo.instance.colorGroups.Count; i++)
        {
            ItemSelection<Color> colorGroup = PlayerInfo.instance.colorGroups[i];
            GridLayoutGroup colorSelectGroup = colorSelectGroupList[i];
            //Create character select buttons
            foreach (Color color in colorGroup.itemList)
            {
                GameObject btnColorSel = Instantiate(colorSelectButtonPrefab);
                btnColorSel.transform.parent = colorSelectGroup.transform;
                btnColorSel.transform.localScale = Vector3.one;
                Image img = btnColorSel.GetComponent<Image>();
                img.color = color;
                Button btn = btnColorSel.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    colorGroup.SelectedItem = color;
                });
            }
        }
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
