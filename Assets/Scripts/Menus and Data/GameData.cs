using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour // game data is going to store wins and stuff
{
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
