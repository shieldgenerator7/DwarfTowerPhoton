

using UnityEngine;

public struct RuleContext
{
    //Self
    public GameObject self;
    //Movement
    public Vector2 targetDir;
    public Vector2 targetPos;
    //OnHit
    public GameObject target;
    public bool isTrigger;
    public bool isCollision;
    //Update
    public float deltaTime;

    public RuleContext(RuleContext template)
    {
        //Self
        this.self = template.self;
        //Movement
        this.targetDir = template.targetDir;
        this.targetPos = template.targetPos;
        //OnHit
        this.target = template.target;
        this.isTrigger = template.isTrigger;
        this.isCollision = template.isCollision;
        //Update
        this.deltaTime = template.deltaTime;
    }
}
