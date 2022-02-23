

using UnityEngine;

public struct RuleContext
{
    //Self
    public ComponentContext componentContext;
    public RuleSet currentRuleSet;
    //Movement
    public Vector2 targetDir;
    public Vector2 targetPos;
    //OnHit
    public GameObject target;
    public bool isTrigger;
    public bool isCollision;
    //Update
    public float deltaTime;
    //Input
    public InputState inputState;
    //Multiplier
    public float statMultiplier;

    public RuleContext(RuleContext template)
    {
        //Self
        this.componentContext = template.componentContext;
        this.currentRuleSet = template.currentRuleSet;
        //Movement
        this.targetDir = template.targetDir;
        this.targetPos = template.targetPos;
        //OnHit
        this.target = template.target;
        this.isTrigger = template.isTrigger;
        this.isCollision = template.isCollision;
        //Update
        this.deltaTime = template.deltaTime;
        //Input
        this.inputState = template.inputState;
        //Multiplier
        this.statMultiplier = template.statMultiplier;
    }
}
