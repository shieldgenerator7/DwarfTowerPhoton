using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityContextDisplay : MonoBehaviour
{
    public AbilityContext abilityContext;

    public Sprite ability1Sprite;
    public Sprite ability2Sprite;
    public Sprite ability3Sprite;
    public string reloadText = "RELOAD";

    public Color playerColor { get; private set; } = Color.white;

    private PlayerController playerController;
    private AbilityContextMenuDisplay acmd;

    // Start is called before the first frame update
    void Start()
    {
        playerController = gameObject.FindComponent<PlayerController>();
        playerController.onAbilityContextChanged += checkAbilityContext;
        playerColor = playerController.gameObject.FindComponent<SpriteRenderer>().color;
        acmd = FindObjectOfType<AbilityContextMenuDisplay>();
    }

    void checkAbilityContext(AbilityContext context)
    {
        acmd.showMenu(
            this,
            context == this.abilityContext
            );
    }

}
