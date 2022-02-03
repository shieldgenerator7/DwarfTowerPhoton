using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSwitcher : MonoBehaviour
{
    public GameObject mnuMain;
    public GameObject mnuCharacterSelect;

    private List<GameObject> menuList = new List<GameObject>();

    private void Awake()
    {
        menuList.Add(mnuMain);
        menuList.Add(mnuCharacterSelect);
    }

    public enum MainMenuOptions
    {
        MAIN,
        CHARACTER_SELECT,
    }
    

    public void switchMenu(MainMenuOptions mmo)
    {
        //Deactivate all menus
        menuList.ForEach(menu => menu.SetActive(false));
        //Activate selected menu
        switch (mmo)
        {
            case MainMenuOptions.MAIN:
                mnuMain.SetActive(true);
                break;
            case MainMenuOptions.CHARACTER_SELECT:
                mnuCharacterSelect.SetActive(true);
                break;
            default:
                throw new System.ArgumentException($"Options not recognized: {mmo}");
        }
    }
}
