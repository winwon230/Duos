using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public enum GameMode
    {
        VBTutorial, VBFriendly, VBPractice, VBTournament 
    }

    public GameMode currentGameMode;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        currentGameMode = GameMode.VBFriendly;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
