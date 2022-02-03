using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSwitcher : MonoBehaviour
{
    public List<GameObject> menuList;    

    public void switchMenu(MonoBehaviour menu)
    {
        //Deactivate all menus
        menuList.ForEach(menu => menu.SetActive(false));
        //Activate selected menu
        menu.gameObject.SetActive(true);
        if (!menuList.Contains(menu.gameObject))
        {
            throw new System.ArgumentException(
                $"Menu not in the list! menu: {menu.gameObject.name}"
                );
        }
    }
}
