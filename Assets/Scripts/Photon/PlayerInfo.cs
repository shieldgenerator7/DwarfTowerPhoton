using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=QbSI3Ajscgc

    public static PlayerInfo instance;

    public int mySelectedCharacter;

    public GameObject[] allCharacters;

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(instance.gameObject);
                instance = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (PlayerPrefs.HasKey("MyCharacter"))
        //{
        //    mySelectedCharacter = PlayerPrefs.GetInt("MyCharacter");
        //}
        //else
        //{
        //    mySelectedCharacter = 0;
        //    PlayerPrefs.SetInt("MyCharacter", mySelectedCharacter);
        //}
    }
}
