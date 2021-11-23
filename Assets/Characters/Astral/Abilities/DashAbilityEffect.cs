using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbilityEffect : MonoBehaviour
{
    public DashAbility dashAbility;

    private Vector2 startPos;
    private Vector2 endPos;

    // Start is called before the first frame update
    void Start()
    {
        //Break away from parent
        transform.parent = null;
        gameObject.name = "Astral " + gameObject.name;
        //Register delegate
        dashAbility.onDash += updateDash;
        updateDash(false);
    }

    void updateDash(bool dash)
    {
        if (dash)
        {
            startPos = dashAbility.rb2d.transform.position;
            Update();
        }
        gameObject.SetActive(dash);
    }

    private void Update()
    {
        transform.position = startPos;
        endPos = dashAbility.rb2d.transform.position;
        Vector2 dir = endPos - startPos;
        transform.up = dir;
        Vector3 scale = transform.localScale;
        scale.y = dir.magnitude;
        transform.localScale = scale;
    }
}
