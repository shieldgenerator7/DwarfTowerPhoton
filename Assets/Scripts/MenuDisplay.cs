using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDisplay : MonoBehaviour
{
    public PlayerInfo playerInfo;
    [Header("Character Select")]
    public Image characterImage;
    public Image characterImageBackground;
    public GridLayoutGroup gridLayoutGroup;
    public GameObject characterSelectButtonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerInfo.onSelectedIndexChanged += updateCharacterImage;
        playerInfo.SelectedIndex = Random.Range(0, playerInfo.allCharacters.Count);
        updateCharacterImage(playerInfo.SelectedIndex);
        //Create character select buttons
        foreach (CharacterInfo charInfo in playerInfo.allCharacters)
        {
            GameObject btnCharSel = Instantiate(characterSelectButtonPrefab);
            btnCharSel.transform.parent = gridLayoutGroup.transform;
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
        RectTransform rect = gridLayoutGroup.GetComponent<RectTransform>();
        int charCount = playerInfo.allCharacters.Count;
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

    void updateCharacterImage(int index)
    {
        CharacterInfo charInfo = playerInfo.allCharacters[index];
        characterImage.sprite = charInfo.sprite;
        characterImage.color = charInfo.defaultColor;
    }
}
