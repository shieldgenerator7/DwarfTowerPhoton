using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabScreenDisplay : MenuDisplay
{
    public GameObject playerIconPrefab;
    public GameObject separatorPrefab;
    public KeyCode activateKeyCode;
    public int redColorGroupIndex = 0;
    public int blueColorGroupIndex = 1;

    private List<GameObject> playerIcons = new List<GameObject>();

    private GridLayoutGroup glg;

    // Start is called before the first frame update
    void Start()
    {
        glg = gameObject.FindComponent<GridLayoutGroup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(activateKeyCode))
        {
            Display(true);
        }
        else if (Input.GetKeyUp(activateKeyCode))
        {
            Display(false);
        }
    }

    public void Display(bool display)
    {
        //Destroy all current icons
        glg.transform.DetachChildren();
        playerIcons.ForEach(pi => Destroy(pi));
        playerIcons.Clear();
        //Create new icons
        if (display)
        {
            List<TeamTokenCaptain> ttcList = FindObjectsOfType<TeamTokenCaptain>().ToList();
            List<PlayerController> pcList = FindObjectsOfType<PlayerController>().ToList();
            //Red team
            TeamTokenCaptain ttcRed = ttcList.Find(
                ttc => ttc.colorGroupIndex == redColorGroupIndex
                );
            pcList.FindAll(
                pc => ttcRed.onSameTeam(TeamToken.getTeamToken(pc.gameObject))
                )
                .ForEach(pc => playerIcons.Add(CreatePlayerIcon(pc)));
            //Separator
            playerIcons.Add(Instantiate(separatorPrefab));
            //Blue team
            TeamTokenCaptain ttcBlue = ttcList.Find(
                ttc => ttc.colorGroupIndex == blueColorGroupIndex
                );
            pcList.FindAll(
                pc => ttcBlue.onSameTeam(TeamToken.getTeamToken(pc.gameObject))
                )
                .ForEach(pc => playerIcons.Add(CreatePlayerIcon(pc)));
            //Layout icons
            playerIcons.ForEach(pi => pi.transform.parent = glg.transform);
        }
    }

    private GameObject CreatePlayerIcon(PlayerController playerController)
    {
        CharacterInfo charInfo = playerController.characterInfo;
        PlayerIcon playerIcon = Instantiate(playerIconPrefab).FindComponent<PlayerIcon>();
        //Image
        Image image = playerIcon.imgCharacterIcon;
        image.color = playerController.playerColor;
        image.sprite = charInfo.sprite;
        //Char Name
        TMP_Text txtCharName = playerIcon.txtCharacterName;
        txtCharName.text = charInfo.characterName;
        //Player Name
        TMP_Text txtPlayerName = playerIcon.txtPlayerName;
        txtPlayerName.text = $"({PlayerInfo.instance.playerName})";
        return playerIcon.gameObject;
    }
}
