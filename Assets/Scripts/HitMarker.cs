using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    [Tooltip("How long to show this hit marker")]
    public float showDuration = 0.1f;

    private PlayerController player;
    public PlayerController Player
    {
        get => player;
        set
        {
            if (player)
            {
                player.onDamagedPlayer -= OnPlayerDealtDamage;
            }
            player = value;
            if (player)
            {
                player.onDamagedPlayer += OnPlayerDealtDamage;
                image.color = player.playerColor;
            }
        }
    }

    private float showStartTime = -1;

    private Image image;

    private void Start()
    {
        image = gameObject.FindComponent<Image>();
        ShowMarker(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
        if (showStartTime >= 0 && Time.time >= showStartTime + showDuration)
        {
            showStartTime = -1;
            this.enabled = false;
            ShowMarker(false);
        }
    }

    public void OnPlayerDealtDamage()
    {
        showStartTime = Time.time;
        this.enabled = true;
        ShowMarker(true);
    }

    private void ShowMarker(bool show)
    {
        image.enabled = show;
    }
}
