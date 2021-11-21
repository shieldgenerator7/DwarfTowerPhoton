using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{//2019-03-15: made by following this tutorial: https://www.youtube.com/watch?v=QbSI3Ajscgc

    public static PlayerInfo instance;

    [SerializeField]
    private int mySelectedCharacter;
    public int SelectedIndex
    {
        get => mySelectedCharacter;
        set
        {
            mySelectedCharacter = Mathf.Clamp(value, 0, allCharacters.Length - 1);
            onSelectedIndexChanged?.Invoke(mySelectedCharacter);
        }
    }
    public delegate void OnSelectedIndexChanged(int index);
    public event OnSelectedIndexChanged onSelectedIndexChanged;

    public GameObject[] allCharacters;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
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
