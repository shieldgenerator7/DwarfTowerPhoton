using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityContextMenuDisplay : MonoBehaviour
{
    public List<Image> separators;
    public List<Image> abilityIcons;

    private AbilityContextDisplay acd;

    // Start is called before the first frame update
    void Start()
    {
        //Check to make sure there are 3 abilityIcons
        if (abilityIcons.Count != 3)
        {
            throw new UnityException("AbilityIcons list should have three images in it!");
        }
        //Hide menu
        showMenu(null, false);
    }

    public void showMenu(AbilityContextDisplay acd, bool show)
    {
        if (show && !acd)
        {
            throw new UnityException("Can't ask to show a null AbilityContextDisplay! acd: " + acd);
        }
        if (show)
        {
            this.acd = acd;
        }
        if (acd == this.acd || !this.acd)
        {
            separators.ForEach(img => img.enabled = show);
            abilityIcons.ForEach(img => img.enabled = show);
            if (show)
            {
                abilityIcons[0].sprite = acd.ability1Sprite;
                abilityIcons[1].sprite = acd.ability2Sprite;
                abilityIcons[2].sprite = acd.ability3Sprite;
                abilityIcons.ForEach(img => img.color = acd.playerColor);
            }
            else
            {
                this.acd = null;
            }
        }
    }
}
