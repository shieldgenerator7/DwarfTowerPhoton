using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterUnlocker : MonoBehaviour
{
    public CharacterInfo character;
    public GameObject unlockNotificationPrefab;
    public string cheatKeyString;
    private string firstLetter;

    private void Start()
    {
        if (PlayerInfo.instance.allCharacters.Contains(character))
        {
            Destroy(this);
        }
        firstLetter = cheatKeyString.Substring(0, 1);
    }

    private void Update()
    {
        if (Input.GetKey(firstLetter))
        {
            cheatKeyString = cheatKeyString.Substring(1);
            firstLetter = cheatKeyString.Substring(0, 1);
        }
        if (cheatKeyString == "")
        {
            Unlock();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController playerController = collision.gameObject.FindComponent<PlayerController>();
        if (playerController && playerController.PV.IsMine)
        {
            if (!PlayerInfo.instance.allCharacters.Contains(character))
            {
                Unlock();
            }
        }
    }

    private void Unlock()
    {
        //Unlock
        PlayerInfo.instance.allCharacters.Add(character);
        //Show notification
        GameObject notification = Instantiate(unlockNotificationPrefab);
        notification.FindComponent<TMP_Text>().text += character.characterName;
        //Destroy this
        Destroy(this);
    }
}
