using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSwitcher : MonoBehaviour
{
    public List<MenuDisplay> menuList;

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
