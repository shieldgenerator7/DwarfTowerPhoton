using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterUnlocker : MonoBehaviour
{
    public CharacterInfo character;
    public GameObject unlockNotificationPrefab;
    public string cheatKeyString;
    public bool unlockOnContact = true;
    private string firstLetter;

    private void Start()
    {
        CheckForCharacterDefaults();
        CheckForCheatStringDefaults();
    }

    private void Update()
    {
        if (!character)
        {
            CheckForCharacterDefaults();
        }
        else
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (unlockOnContact)
        {
            CheckUnlock(collision.gameObject);
        }
    }

    public void CheckUnlock(GameObject go)
    {
        PlayerController playerController = go.FindComponent<PlayerController>();
        if (playerController && playerController.PV.IsMine)
        {
            if (!PlayerInfo.instance.unlockedCharacters.Contains(character))
            {
                Unlock();
            }
        }
    }

    private void Unlock()
    {
        //Unlock
        PlayerInfo.instance.unlockedCharacters.Add(character);
        //Show notification
        GameObject notification = Instantiate(unlockNotificationPrefab);
        notification.FindComponent<TMP_Text>().text += character.characterName;
        //Destroy this
        Destroy(this);
    }

    private void CheckForCharacterDefaults()
    {
        //Character default
        if (!character)
        {
            PlayerController playerController = gameObject.FindComponent<PlayerController>();
            if (playerController)
            {
                character = playerController.characterInfo;
                CheckForCheatStringDefaults();
            }
        }
        //Destroy if already unlocked
        if (character && PlayerInfo.instance.unlockedCharacters.Contains(character))
        {
            Destroy(this);
        }
    }
    private void CheckForCheatStringDefaults()
    {
        //Cheat key string
        if (character && string.IsNullOrEmpty(cheatKeyString))
        {
            cheatKeyString = character.characterName;
        }
        if (!string.IsNullOrEmpty(cheatKeyString))
        {
            firstLetter = cheatKeyString.Substring(0, 1);
        }
    }
}
