using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDisplay : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public Image characterImage;
    public List<Image> characterImageList;

    // Start is called before the first frame update
    void Start()
    {
        playerInfo.onSelectedIndexChanged += updateCharacterImage;
        playerInfo.SelectedIndex = Random.Range(0, playerInfo.allCharacters.Length);
        updateCharacterImage(playerInfo.SelectedIndex);
    }

    void updateCharacterImage(int index)
    {
        characterImage.sprite = characterImageList[index].sprite;
        characterImage.color = characterImageList[index].color;
    }
}
