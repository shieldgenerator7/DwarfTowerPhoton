using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanController : MonoBehaviour
{
    public float maxMoveSpeed = 3;
    private float moveSpeed;
    private Vector2 direction;

    private Dictionary<TeamToken, float> teamCaptains = new Dictionary<TeamToken, float>();

    private Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        foreach (TeamToken tt in FindObjectsOfType<TeamToken>())
        {
            if (!teamCaptains.ContainsKey(tt.teamCaptain))
            {
                teamCaptains.Add(tt.teamCaptain, 0);
            }
        }
        updateDirection();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TeamToken tt = TeamToken.getTeamToken(collision.gameObject);
        if (tt && tt.isPlayer())
        {
            updateDirection(tt.teamCaptain, 1);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        TeamToken tt = TeamToken.getTeamToken(collision.gameObject);
        if (tt && tt.isPlayer())
        {
            updateDirection(tt.teamCaptain, -1);
        }
    }

    void updateDirection(TeamToken teamCaptain, float amount)
    {
        teamCaptains[teamCaptain] += amount;
        updateDirection();
    }
    void updateDirection()
    {
        direction = Vector2.zero;
        foreach (TeamToken tt in teamCaptains.Keys)
        {
            direction += (Vector2)(transform.position - tt.transform.position).normalized
                * teamCaptains[tt];
        }
        float magnitude = direction.magnitude;
        magnitude = Mathf.Clamp(magnitude, 0, maxMoveSpeed);
        rb2d.velocity = direction.normalized * magnitude;
    }
}
