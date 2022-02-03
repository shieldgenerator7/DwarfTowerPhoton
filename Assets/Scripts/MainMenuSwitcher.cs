using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSwitcher : MonoBehaviour
{
    [Tooltip("The list of menu objects. The first one in the list will be shown first")]
    public List<MenuDisplay> menuList;

    private void Awake()
    {
        switchMenu(menuList[0]);
    }

    public void switchMenu(MenuDisplay menu)
    {
        //Deactivate all menus
        menuList.ForEach(menu => menu.gameObject.SetActive(false));
        //Activate selected menu
        menu.gameObject.SetActive(true);
        if (!menuList.Contains(menu))
        {
            throw new System.ArgumentException(
                $"Menu not in the list! menu: {menu.gameObject.name}"
                );
        }
    }
}
